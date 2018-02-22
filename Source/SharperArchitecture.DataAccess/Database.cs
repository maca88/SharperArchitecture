using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Web;
using FluentNHibernate;
using FluentNHibernate.Mapping;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Common.Configuration;
using SharperArchitecture.Common.Events;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Events;
using SharperArchitecture.DataAccess.Interceptors;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Specifications;
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
using SharperArchitecture.Common.Exceptions;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Decorators;

namespace SharperArchitecture.DataAccess
{
    public static class Database
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Database));
        private static readonly ConcurrentDictionary<string, DatabaseConfiguration> RegisteredDatabaseConfigurations = 
            new ConcurrentDictionary<string, DatabaseConfiguration>();
        private static readonly ConcurrentDictionary<ISessionFactory, SessionFactoryInfo> SessionFactories = 
            new ConcurrentDictionary<ISessionFactory, SessionFactoryInfo>();
        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<DatabaseConfiguration>>
            CachedConfigurationsForModels = new ConcurrentDictionary<Type, IReadOnlyCollection<DatabaseConfiguration>>();
        private static readonly Lazy<List<Assembly>> StepAssemblies;

        static Database()
        {
            StepAssemblies = new Lazy<List<Assembly>>(() =>
            {
                return AppConfiguration.GetDomainAssemblies()
                    .Where(assembly => assembly != Assembly.GetAssembly(typeof(IAutomappingConfiguration)))
                    .Where(assembly => assembly != Assembly.GetExecutingAssembly())
                    .Where(assembly => assembly.GetTypes().Any(o => typeof(IAutomappingStep).IsAssignableFrom(o)))
                    .ToList();
            });

        }

        #region Public members

        public static ISessionFactory CreateSessionFactory(
            IEventPublisher eventPublisher,
            DatabaseConfiguration dbConfiguration)
        {
            var cfg = dbConfiguration.NHibernateConfiguration;
            var entityAssemblies = dbConfiguration.EntityAssemblies;
            var conventionAssemblies = dbConfiguration.ConventionAssemblies;

            var automappingConfiguration = dbConfiguration.AutoMappingConfiguration;
            automappingConfiguration.AddStepAssemblies(StepAssemblies.Value);

            var hbmMappingsPath = dbConfiguration.HbmMappingsPath;
            if (!string.IsNullOrEmpty(hbmMappingsPath) && hbmMappingsPath.StartsWith("."))
                hbmMappingsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, hbmMappingsPath));

            try
            {
                var fluentConfig = Fluently.Configure(cfg);
                dbConfiguration.FluentConfigurationAction?.Invoke(fluentConfig);
                var autoPestModel = CreateAutomappings(automappingConfiguration, entityAssemblies,
                    dbConfiguration.Conventions, conventionAssemblies, cfg, eventPublisher);
                fluentConfig
                    .Mappings(m =>
                    {
                        m.AutoMappings.Add(autoPestModel);
                        foreach (var domainAssembly in entityAssemblies)
                        {
                            m.HbmMappings.AddFromAssembly(domainAssembly);
                        }
                    })
                    .ExposeConfiguration(configuration =>
                    {
                        if (configuration.Interceptor == null || configuration.Interceptor.GetType() == typeof(EmptyInterceptor))
                        {
                            configuration.SetInterceptor(new NhibernateInterceptor());
                        }

                        eventPublisher?.Publish(new NhConfigurationEvent(configuration));
                        dbConfiguration.ConfigurationCompletedAction?.Invoke(configuration);

                        RecreateOrUpdateSchema(autoPestModel, configuration, dbConfiguration);
                        if (dbConfiguration.ValidateSchema)
                            ValidateSchema(configuration);

                        if (string.IsNullOrEmpty(hbmMappingsPath))
                        {
                            return;
                        }
                        if (!Directory.Exists(hbmMappingsPath))
                        {
                            Directory.CreateDirectory(hbmMappingsPath);
                        }
                        autoPestModel.WriteMappingsTo(hbmMappingsPath);
                    });

                var sessionFactory = fluentConfig.BuildSessionFactory();
                RegisterSessionFactory(sessionFactory, cfg, autoPestModel, dbConfiguration);
                return sessionFactory;
            }
            catch (FluentConfigurationException e)
            {
                var innerEx = e.InnerException as FluentConfigurationException;
                if (innerEx != null)
                {
                    e = innerEx;
                }
                if (e.PotentialReasons.Any())
                {
                    Logger.Fatal("PotentialReasons: " + string.Join(System.Environment.NewLine, e.PotentialReasons));
                }
                Logger.Fatal(e.InnerException);
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }
            return null;
        }

        public static bool MultipleDatabases => RegisteredDatabaseConfigurations.Count > 1;

        public static bool HasDefaultDatabase => RegisteredDatabaseConfigurations.ContainsKey(DatabaseConfiguration.DefaultName);

        public static bool ContainsDatabaseConfiguration(string name)
        {
            return RegisteredDatabaseConfigurations.ContainsKey(name);
        }

        public static void DropTables(ISessionFactory sessionFactory)
        {
            if(!SessionFactories.ContainsKey(sessionFactory))
                throw new KeyNotFoundException("sessionFactory");
            var info = SessionFactories[sessionFactory];
            var schema = new SchemaExport(info.Configuration);
            schema.Drop(true, true);
        }

        public static void RecreateTables(ISessionFactory sessionFactory)
        {
            if (!SessionFactories.ContainsKey(sessionFactory))
                throw new KeyNotFoundException("sessionFactory");
            var info = SessionFactories[sessionFactory];

            SetupSchema(info.AutoPersistenceModel, info.Configuration,
                (dbConnection, conventions) => RecreateSchema(info.Configuration, dbConnection, conventions));
        }

        internal static SessionFactoryInfo GetSessionFactoryInfo(ISession session)
        {
            return GetSessionFactoryInfo(session.SessionFactory);
        }

        public static IReadOnlyCollection<DatabaseConfiguration> GetDatabaseConfigurationsForModel<T>()
        {
            return GetDatabaseConfigurationsForModel(typeof(T));
        }

        public static IReadOnlyCollection<DatabaseConfiguration> GetDatabaseConfigurationsForModel(Type modelType)
        {
            if (modelType == null)
            {
                throw new ArgumentNullException(nameof(modelType));
            }
            IReadOnlyCollection<DatabaseConfiguration> result;
            if (CachedConfigurationsForModels.TryGetValue(modelType, out result))
            {
                return result;
            }
            var list = new List<DatabaseConfiguration>();

            foreach (var dbConfig in RegisteredDatabaseConfigurations.Values.Where(o => o.EntityAssemblies.Contains(modelType.Assembly)))
            {
                if (dbConfig.AutoMappingConfiguration.ShouldMap(modelType))
                {
                    list.Add(dbConfig);
                }
            }
            result = list.AsReadOnly();
            CachedConfigurationsForModels.TryAdd(modelType, result);
            return result;
        }

        #endregion

        internal static void Cleanup(bool dropTables = true)
        {
            foreach (var sessionFactory in SessionFactories.Keys)
            {
                if (dropTables)
                {
                    DropTables(sessionFactory);
                }
                sessionFactory.Dispose();
            }
            CachedConfigurationsForModels.Clear();
            SessionFactories.Clear();
        }

        internal static SessionFactoryInfo GetSessionFactoryInfo(ISessionFactory sessionFactory)
        {
            SessionFactoryInfo info;
            return SessionFactories.TryGetValue(sessionFactory, out info)
                ? info
                : null;
        }

        internal static void RegisteredDatabaseConfiguration(DatabaseConfiguration dbConfiguration)
        {
            if (CachedConfigurationsForModels.Any())
            {
                throw new SharperArchitectureException("Cannot register a database configuration after GetDatabaseConfigurationsForModel is called");
            }
            RegisteredDatabaseConfigurations.AddOrUpdate(dbConfiguration.Name, dbConfiguration, (k, v) => dbConfiguration);
        }

        internal static IReadOnlyCollection<DatabaseConfiguration> GetRegisteredDatabaseConfigurations()
        {
            return new ReadOnlyCollection<DatabaseConfiguration>(RegisteredDatabaseConfigurations.Values.ToList());
        }

        private static void RegisterSessionFactory(ISessionFactory sessionFactory, Configuration configuration, AutoPersistenceModel autoPersistenceModel, 
            DatabaseConfiguration dbConfiguration)
        {
            var sessionFactoryInfo = new SessionFactoryInfo(sessionFactory, configuration, autoPersistenceModel, dbConfiguration);
            sessionFactoryInfo.ValidateSettings();
            SessionFactories.AddOrUpdate(sessionFactory, sessionFactoryInfo, (k, v) => sessionFactoryInfo);
        }

        private static AutoPersistenceModel CreateAutomappings(IAutomappingConfiguration automappingConfiguration, ICollection<Assembly> assemblies, 
            ConventionsConfiguration conventionsConfiguration, IEnumerable<Assembly> conventionAssemblies,
            Configuration configuration, IEventPublisher eventPublisher)
        {
            var dialect = NHibernate.Dialect.Dialect.GetDialect(configuration.Properties);
            var conventions = new List<IConvention>();
            foreach (var convType in conventionAssemblies
                .Select(a => new
                    {
                        Assembly = a,
                        Order = a.GetCustomAttribute<ConventionAttribute>() != null
                                    ? a.GetCustomAttribute<ConventionAttribute>().Order
                                    : int.MaxValue
                    })
                .OrderBy(a => a.Order)
                .SelectMany(a => a.Assembly.GetTypes()
                    .Where(t => typeof(IConvention).IsAssignableFrom(t))
                    .Where(t => !t.IsInterface && !t.IsAbstract)
                    .Select(t => new
                        {
                            Type = t,
                            Order = t.GetCustomAttribute<ConventionAttribute>() != null
                                        ? t.GetCustomAttribute<ConventionAttribute>().Order
                                        : int.MaxValue
                        })
                    .OrderBy(o => o.Order)
                    .Select(o => o.Type)))
            {
                IConvention conv;
                var constuctor = convType.GetConstructor(new[] { typeof(ConventionsConfiguration) });
                if (constuctor != null)
                    conv = (IConvention) constuctor.Invoke(new object[] {conventionsConfiguration});
                else
                {
                    try
                    {
                        conv = (IConvention) Activator.CreateInstance(convType);
                    }
                    catch (MissingMethodException e)
                    {
                        Logger.WarnFormat("Type '{0}' does not have a parameterless constructor defined. Details: {1}",
                            convType, e);
                        throw;
                    }
                    catch (TargetInvocationException e)
                    {
                        ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                        return null;
                    }
                }
                    
                var dbConv = conv as ISchemaConvention;
                if (dbConv != null)
                {
                    if (!dbConv.CanApply(dialect))
                    {
                        continue;
                    }
                    dbConv.Setup(configuration);
                }
                conventions.Add(conv);
            }

            return GetAutoPersistenceModel(configuration, eventPublisher, automappingConfiguration, assemblies)
                .UseOverridesFromAssemblies(assemblies)
                .AddConventions(conventions)
                .AddFilters(GetFilterDefinitions(assemblies))
                .Conventions.Add(PrimaryKey.Name.Is(o => "Id"))
                //.Conventions.Add(ForeignKey.EndsWith("Id"))
                .Alterations(collection => collection.AddFromAssemblies(assemblies)); //TODO: add new list
        }


        private static IEnumerable<Type> GetFilterDefinitions(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(o => o.GetTypes()).Where(o => typeof (FilterDefinition).IsAssignableFrom(o));
        }

        private static CustomAutoPersistenceModel GetAutoPersistenceModel(Configuration configuration, IEventPublisher eventPublisher, IAutomappingConfiguration cfg, IEnumerable<Assembly> assemblies)
        {
            var model = new CustomAutoPersistenceModel(configuration, eventPublisher, cfg);
            model.AddTypeSource(new CombinedAssemblyTypeSource(assemblies.Select(o => new AssemblyTypeSource(o))));
            return model;
        }

        private static void SetupSchema(AutoPersistenceModel autoPersistenceModel, Configuration config,
            Action<IDbConnection, ICollection<ISchemaConvention>> setupAction)
        {
            DbConnection connection = null;
            IConnectionProvider provider = null;
            var settings = new Dictionary<string, string>();
            var dialect = NHibernate.Dialect.Dialect.GetDialect(config.Properties);
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
                setupAction(connection, schemaConvs);
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

        private static void RecreateOrUpdateSchema(AutoPersistenceModel autoPersistenceModel, Configuration config, DatabaseConfiguration dbConfiguration)
        {
            //Run only if recreate or update db is allowed
            if (!dbConfiguration.RecreateAtStartup && !dbConfiguration.UpdateSchemaAtStartup) return;

            SetupSchema(autoPersistenceModel, config,
                (dbConnection, conventions) =>
                {
                    //Setup schema conventions (this will happen only one time)
                    //foreach (var conv in conventions)
                    //{
                    //    conv.Setup(config);
                    //}

                    if (dbConfiguration.RecreateAtStartup)
                        RecreateSchema(config, dbConnection, conventions);
                    else if (dbConfiguration.UpdateSchemaAtStartup) //Usefull only in develop environment
                        UpdateSchema(config, dbConnection, conventions);
                });
        }


        private static void RecreateSchema(Configuration config, IDbConnection dbConnection,
            ICollection<ISchemaConvention> conventions, Action beforeCreateOrUpdateAction = null)
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

            beforeCreateOrUpdateAction?.Invoke();

            schema.Execute(beforeExecuteSql, afterExecuteSql, true, true, dbConnection, null); //Drop
            schema.Execute(beforeExecuteSql, afterExecuteSql, true, false, dbConnection, null); //Create

            //after schema create
            foreach (var createShema in conventions.OfType<ICreateSchemaConvention>())
            {
                createShema.ApplyAfterSchemaCreate(config, dbConnection);
            }
        }

        private static void UpdateSchema(Configuration config, IDbConnection dbConnection,
            ICollection<ISchemaConvention> conventions, Action beforeCreateOrUpdateAction = null)
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

            beforeCreateOrUpdateAction?.Invoke();

            schema.Execute(beforeExecuteSql, afterExecuteSql, dbConnection, true);

            //after schema update
            foreach (var updateShema in conventions.OfType<IUpdateSchemaConvention>())
            {
                updateShema.ApplyAfterSchemaUpdate(config, dbConnection);
            }
        }

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
