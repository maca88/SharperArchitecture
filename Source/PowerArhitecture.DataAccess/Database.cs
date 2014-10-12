using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;
using PowerArhitecture.DataAccess.Settings;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Tool.hbm2ddl;
using log4net;

namespace PowerArhitecture.DataAccess
{
    public class Database
    {
        private NHibernate.Caches.SysCache.SysCache _dummyCache; //Only here because it needs to be copied in bin folder
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Database));
        private static readonly Dictionary<ISessionFactory, SessionFactoryInfo> SessionFactories = new Dictionary<ISessionFactory, SessionFactoryInfo>();
        private static IEventAggregator _eventAggregator;

        static Database(){}

        public static ISessionFactory CreateSessionFactory(Configuration configuration, IEventAggregator eventAggregator, 
            IDatabaseSettings databaseSettings)
        {
            return CreateSessionFactory(
                configuration,
                eventAggregator,
                databaseSettings,
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(assembly => assembly.GetTypes().Any(o => (typeof(IEntity)).IsAssignableFrom(o))).ToList(),
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(assembly => assembly != Assembly.GetAssembly(typeof(IAutomappingConfiguration)))
                    .Where(assembly => assembly.GetTypes().Any(o => (typeof(IConvention)).IsAssignableFrom(o))).ToList(),
                new AutomappingConfiguration()
                    .AddStepAssemblies(AppDomain.CurrentDomain.GetAssemblies()
                        .Where(assembly => assembly != Assembly.GetAssembly(typeof(IAutomappingConfiguration)))
                        .Where(assembly => assembly != Assembly.GetExecutingAssembly())
                        .Where(assembly => assembly.GetTypes().Any(o => (typeof(IAutomappingStep)).IsAssignableFrom(o))).ToList()),
                HttpRuntime.AppDomainAppPath,
                true,
                null);
        }

        public static ISessionFactory CreateSessionFactory(
            Configuration cfg,
            IEventAggregator eventAggregator,
            IDatabaseSettings dbSettings, 
            ICollection<Assembly> entityAssemblies,
            IEnumerable<Assembly> conventionAssemblies,
            IAutomappingConfiguration automappingConfiguration, 
            string hbmMappingsPath, 
            bool xmlConfig,
            Action<FluentConfiguration> configureAction)
        {
            try
            {
                _eventAggregator = eventAggregator;
                if (xmlConfig) //configure with xml
                    cfg.Configure();
                var fluentConfig = Fluently.Configure(cfg);
                if (configureAction != null)
                    configureAction(fluentConfig);
                var dialect = NHibernate.Dialect.Dialect.GetDialect(cfg.Properties);
                var autoPestModel = CreateAutomappings(automappingConfiguration, entityAssemblies, dbSettings.Conventions, conventionAssemblies, dialect);
                fluentConfig
                    .Mappings(m =>
                        {
                            m.AutoMappings.Add(autoPestModel);
                            foreach (var domainAssembly in entityAssemblies)
                            {
                                m.HbmMappings.AddFromAssembly(domainAssembly);
                            }
                            var mappingsDirecotry = Path.Combine(hbmMappingsPath, "Mappings");
                            if (!Directory.Exists(mappingsDirecotry))
                                Directory.CreateDirectory(mappingsDirecotry);
                            m.AutoMappings.ExportTo(mappingsDirecotry);
                            m.FluentMappings.ExportTo(mappingsDirecotry);
                        })
                    .ExposeConfiguration(configuration =>
                        {
                            if (_eventAggregator != null)
                                _eventAggregator.SendMessage(new NhConfigurationEvent(configuration));
                            ConfigureEnvers(configuration, entityAssemblies);
                            //ConfigureNhibernateValidator(configuration, entityAssemblies);
                            RecreateOrUpdateSchema(autoPestModel, configuration, dbSettings, 
                                NHibernate.Dialect.Dialect.GetDialect(configuration.Properties));
                            if(dbSettings.ValidateSchema)
                                ValidateSchema(configuration);
                        });
                
                var sessionFactory = fluentConfig.BuildSessionFactory();
                RegisterSessionFactory(sessionFactory, cfg, dbSettings);
                return sessionFactory;
            }
            catch (FluentConfigurationException e)
            {
                Logger.Fatal(e);
                throw;
            }
        }

        public static void DropTables(ISessionFactory sessionFactory)
        {
            if(!SessionFactories.ContainsKey(sessionFactory))
                throw new KeyNotFoundException("sessionFactory");
            var info = SessionFactories[sessionFactory];
            var schema = new SchemaExport(info.Configuration);
            schema.Drop(true, true);
        }

        public static SessionFactoryInfo GetSessionFactoryInfo(ISession session)
        {
            return GetSessionFactoryInfo(session.SessionFactory);
        }

        public static SessionFactoryInfo GetSessionFactoryInfo(ISessionFactory sessionFactory)
        {
            return SessionFactories[sessionFactory];
        }

        private static void RegisterSessionFactory(ISessionFactory sessionFactory, Configuration configuration, IDatabaseSettings dbSettings)
        {
            var sessionFactoryInfo = new SessionFactoryInfo(sessionFactory, configuration);
            sessionFactoryInfo.ValidateSettings(dbSettings);
            SessionFactories.Add(sessionFactory, sessionFactoryInfo);
        }

        private static AutoPersistenceModel CreateAutomappings(IAutomappingConfiguration automappingConfiguration,
            ICollection<Assembly> assemblies, ConventionsSettings conventionsSettings, IEnumerable<Assembly> conventionAssemblies,
            NHibernate.Dialect.Dialect dialect)
        {
            var conventions = new List<IConvention>();
            foreach (var convType in conventionAssemblies
                .Select(a => new
                    {
                        Assembly = a,
                        Order = a.GetCustomAttribute<ConventionsAttribute>() != null
                                    ? a.GetCustomAttribute<ConventionsAttribute>().Order
                                    : int.MaxValue
                    })
                .OrderBy(a => a.Order)
                .SelectMany(a => a.Assembly.GetTypes()
                    .Where(t => typeof(IConvention).IsAssignableFrom(t))
                    .Where(t => !t.IsInterface && !t.IsAbstract)
                    .Select(t => new
                        {
                            Type = t,
                            Order = t.GetCustomAttribute<ConventionsAttribute>() != null
                                        ? t.GetCustomAttribute<ConventionsAttribute>().Order
                                        : int.MaxValue
                        })
                    .OrderBy(o => o.Order)
                    .Select(o => o.Type)))
            {
                IConvention conv;
                var constuctor = convType.GetConstructor(new[] { typeof(ConventionsSettings) });
                if (constuctor != null)
                    conv = (IConvention) constuctor.Invoke(new object[] {conventionsSettings});
                else
                {
                    try
                    {
                        conv = (IConvention) Activator.CreateInstance(convType);
                    }
                    catch (MissingMethodException e)
                    {
                        Logger.WarnFormat("Type '{0}' does not have a parameterless constructor defined. Details: {1}", convType, e);
                        throw;
                    }
                }
                    
                var dbConv = conv as ISchemaConvention;
                if (dbConv != null && !dbConv.CanApply(dialect)) continue;
                conventions.Add(conv);
            }

            return AutoMap
                .Assemblies(automappingConfiguration, assemblies)
                .UseOverridesFromAssemblies(assemblies)
                .AddConventions(conventions)
                .Conventions.Add(PrimaryKey.Name.Is(o => "Id"))
                .Conventions.Add(ForeignKey.EndsWith("Id"))
                .Alterations(collection => collection.AddFromAssemblies(assemblies)); //TODO: add new list
        }

        private static void RecreateOrUpdateSchema(AutoPersistenceModel autoPersistenceModel, Configuration config, IDatabaseSettings dbSettings, 
            NHibernate.Dialect.Dialect dialect, Action beforeCreateOrUpdateAction = null)
        {
            //Run only if recreate or update db is allowed
            if (!dbSettings.RecreateAtStartup && !dbSettings.UpdateSchemaAtStartup) return;

            IDbConnection connection = null;
            IConnectionProvider provider = null;
            var settings = new Dictionary<string, string>();
            foreach (var pair in dialect.DefaultProperties)
            {
                settings[pair.Key] = pair.Value;
            }
            if (config.Properties != null)
            {
                foreach (var pair2 in config.Properties)
                {
                    settings[pair2.Key] = pair2.Value;
                }
            }
            try
            {
                provider = ConnectionProviderFactory.NewConnectionProvider(settings);
                connection = provider.GetConnection();
                SchemaMetadataUpdater.QuoteTableAndColumns(config);
                var schemaConvs = autoPersistenceModel.Conventions.Find<ISchemaConvention>().ToList();
                if (dbSettings.RecreateAtStartup)
                    RecreateSchema(config, connection, schemaConvs, beforeCreateOrUpdateAction);
                else if (dbSettings.UpdateSchemaAtStartup) //Usefull only in develop environment
                    UpdateSchema(config, connection, schemaConvs, beforeCreateOrUpdateAction);
            }
            catch (HibernateException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new HibernateException(exception.Message, exception);
            }
            finally
            {
                if (connection != null)
                {
                    provider.CloseConnection(connection);
                    provider.Dispose();
                }
            }
        }

        /*
        private static void SubscribeEventListeners(Configuration config)
        {
            config.SetListener(ListenerType.Save, new NhSaveEventListener()); //TODO: envers
            config.SetListener(ListenerType.SaveUpdate, new NhSaveOrUpdateEventListener());
            config.SetListener(ListenerType.PreUpdate, new PreUpdateEventListener());
            //config.SetListener(ListenerType.PreInsert, new PreInsertEventListener());
        }*/

        private static void RecreateSchema(Configuration config, IDbConnection dbConnection,
            ICollection<ISchemaConvention> conventions, Action beforeCreateOrUpdateAction)
        {
            var schema = new SchemaExportExt(config);
            Action<IDbCommand> beforeExecuteSql = dbCommand =>
            {
                foreach (var dbConv in conventions)
                {
                    dbConv.ApplyBeforeExecutingQuery(config, dbConnection, dbCommand);
                }
            };
            Action<IDbCommand> afterExecuteSql = dbCommand =>
            {
                foreach (var dbConv in conventions)
                {
                    dbConv.ApplyAfterExecutingQuery(config, dbConnection, dbCommand);
                }
            };

            //before schema create
            foreach (var createShema in conventions.OfType<ICreateSchemaConvention>())
            {
                createShema.ApplyBeforeSchemaCreate(config, dbConnection);
            }

            if (beforeCreateOrUpdateAction != null)
                beforeCreateOrUpdateAction();

            schema.Execute(beforeExecuteSql, afterExecuteSql, true, true, dbConnection, null); //Drop
            schema.Execute(beforeExecuteSql, afterExecuteSql, true, false, dbConnection, null); //Create

            //after schema create
            foreach (var createShema in conventions.OfType<ICreateSchemaConvention>())
            {
                createShema.ApplyAfterSchemaCreate(config, dbConnection);
            }
        }

        private static void UpdateSchema(Configuration config, IDbConnection dbConnection,
            ICollection<ISchemaConvention> conventions, Action beforeCreateOrUpdateAction)
        {
            var schema = new SchemaUpdateExt(config);
            Action<IDbCommand> beforeExecuteSql = dbCommand =>
            {
                foreach (var dbConv in conventions)
                {
                    dbConv.ApplyBeforeExecutingQuery(config, dbConnection, dbCommand);
                }
            };
            Action<IDbCommand> afterExecuteSql = dbCommand =>
            {
                foreach (var dbConv in conventions)
                {
                    dbConv.ApplyAfterExecutingQuery(config, dbConnection, dbCommand);
                }
            };
            //before schema update
            foreach (var updateShema in conventions.OfType<IUpdateSchemaConvention>())
            {
                updateShema.ApplyBeforeSchemaUpdate(config, dbConnection);
            }

            if (beforeCreateOrUpdateAction != null)
                beforeCreateOrUpdateAction();

            schema.Execute(beforeExecuteSql, afterExecuteSql, dbConnection, true);

            //after schema update
            foreach (var updateShema in conventions.OfType<IUpdateSchemaConvention>())
            {
                updateShema.ApplyAfterSchemaUpdate(config, dbConnection);
            }
        }

        private static void ConfigureEnvers(Configuration config, IEnumerable<Assembly> entityAssemblies)
        {
            var enversConfig = new NHibernate.Envers.Configuration.Fluent.FluentConfiguration();
            var types = entityAssemblies
                .SelectMany(o => o.GetTypes()
                                  .Where(t => typeof (IRevisionedEntity).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType))
                .ToList();

            if (!types.Any()) return;
            enversConfig.Audit(types);
            config.IntegrateWithEnvers(enversConfig);
        }

        /*
        private static void ConfigureNhibernateValidator(Configuration config, IEnumerable<Assembly> entityAssemblies)
        {
            var nhvConfiguration = new NHibernate.Validator.Cfg.Loquacious.FluentConfiguration();
            nhvConfiguration
               .SetDefaultValidatorMode(ValidatorMode.OverrideExternalWithAttribute)
               .Register(entityAssemblies.SelectMany(o => o.ValidationDefinitions()))
               .IntegrateWithNHibernate
                   .ApplyingDDLConstraints()
                   .RegisteringListeners();
            var provider = NHibernate.Validator.Cfg.Environment.SharedEngineProvider; //Was initialized in a startup task
            var validatorEngine = provider.GetEngine();
            validatorEngine.Configure(nhvConfiguration);
            config.Initialize(validatorEngine);
        }*/

        private static void ValidateSchema(Configuration config)
        {
            var validator = new SchemaValidator(config);
            try
            {
                validator.Validate();
            }
            catch (HibernateException e)
            {
                Logger.Fatal(e);
                throw;
            }
        }
    }
}
