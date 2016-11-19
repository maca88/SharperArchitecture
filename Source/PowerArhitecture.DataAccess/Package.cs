using System;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Event;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.SimpleInjector;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Decorators;
using PowerArhitecture.DataAccess.EventListeners;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Factories;
using PowerArhitecture.DataAccess.NHEventListeners;
using PowerArhitecture.DataAccess.Providers;
using PowerArhitecture.DataAccess.Specifications;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Diagnostics;
using SimpleInjector.Extensions;
using SimpleInjector.Packaging;

namespace PowerArhitecture.DataAccess
{
    public class Package : IPackage
    {
        private const string ResolvingKeyedRepositoryKey = "ResolvingKeyedRepository";

        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

            var registration = Lifestyle.Transient.CreateRegistration<UnitOfWork>(container);
            container.AddRegistration(typeof(UnitOfWork), registration);
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                    "UnitOfWork wraps an execution context scope and must be manually disposed");
            
            // As Simple Injector does not support injection of primitive values so the workaround is to set it manually after the creation
            container.RegisterWithContext<IUnitOfWork>(ctx =>
            {
                var attr = ctx.Parameter?.GetCustomAttribute<IsolationLevelAttribute>();
                var instance = container.GetInstance<UnitOfWork>();
                if (attr != null)
                {
                    instance.IsolationLevel = attr.Level;
                }
                return instance;
            }, r =>
            {
                r.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                    "UnitOfWork wraps an execution context scope and must be manually disposed");
            });

            //Events
            registration = Lifestyle.Singleton.CreateRegistration<NhSaveOrUpdateEventListener>(container);
            container.AppendToCollection(typeof(ISaveOrUpdateEventListener), registration);
            registration = Lifestyle.Singleton.CreateRegistration<NhUpdateEventListener>(container);
            container.AppendToCollection(typeof(ISaveOrUpdateEventListener), registration);
            registration = Lifestyle.Singleton.CreateRegistration<NhSaveEventListener>(container);
            container.AppendToCollection(typeof(ISaveOrUpdateEventListener), registration);

            registration = Lifestyle.Singleton.CreateRegistration<EntityPreUpdateInsertEventListener>(container);
            container.AppendToCollection(typeof(IPreInsertEventListener), registration);
            container.AppendToCollection(typeof(IPreUpdateEventListener), registration);

            // Register a empty collection for each NHibernate event so that NhConfigurationEventHandler will work
            container.RegisterCollection<IFlushEventListener>();
            container.RegisterCollection<IDeleteEventListener>();
            container.RegisterCollection<IAutoFlushEventListener>();
            container.RegisterCollection<IPreDeleteEventListener>();
            container.RegisterCollection<IPostInsertEventListener>();
            container.RegisterCollection<IPostUpdateEventListener>();
            container.RegisterCollection<IPostCollectionUpdateEventListener>();
            container.RegisterCollection<IPostDeleteEventListener>();

            registration = Lifestyle.Singleton.CreateRegistration<AuditEntityEventListener>(container);
            container.AppendToCollection(typeof(IPreUpdateEventListener), registration);

            container.RegisterSingleton<IAuditUserProvider, AuditUserProvider>();

            // Repositories - they must be transient in order to work with multiple databases as keyed repository registrations are 
            // just decorated non keyed where different the session is set
            AppConfiguration.GetDomainAssemblies()
                .SelectMany(o => o.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && typeof(IRepository).IsAssignableFrom(t))
                .Select(t => new {
                    Implementation = t,
                    Services = t.GetInterfaces().Where(o => typeof(IRepository) != o && typeof(IRepository).IsAssignableFrom(o))
                })
                .ForEach(o =>
                {
                    registration = Lifestyle.Transient.CreateRegistration(o.Implementation, container);
                    foreach (var serviceType in o.Services)
                    {
                        container.AddRegistration(serviceType, registration);
                    }
                });
            container.RegisterConditional(typeof(IRepository<>), typeof(Repository<>), Lifestyle.Transient, o => !o.Handled);
            container.RegisterConditional(typeof(IRepository<,>), typeof(Repository<,>), Lifestyle.Transient, o => !o.Handled);

            // When we have multiple databases we need to register a special registration that knows which session to inject.
            // Skip if the developer did not set the KeyedDependencyInjectionBehavior as it is not needed where only one database exists
            var keyedDep = container.Options.DependencyInjectionBehavior as KeyedDependencyInjectionBehavior;
            if (keyedDep != null)
            {
                RegisterKeyedRegistrationNotFound(keyedDep, container);

                // The initializer is responsable to verify if the database configuration name is valid for the given model. 
                // Used only if there are multiple databases registered
                // The initializer also modifies the repository session if it detect that the model belongs to a different session factory, 
                // this is done only when there is no explicit database configuration name set
                container.RegisterInitializer(data =>
                {
                    // Try get the database configuration name from the scope in case there is a keyed repository resolvning going on
                    var scope = Lifestyle.Scoped.GetCurrentScope(container);
                    var dbConfigName = scope?.GetItem(ResolvingKeyedRepositoryKey) as string;
                    var explicitDbConfigName = true;
                    dynamic repo = data.Instance;
                    if (dbConfigName == null)
                    {
                        // Skip if the session is not decorated
                        repo = data.Instance;
                        var decSession = repo.Session as SessionDecorator;
                        if (decSession == null)
                        {
                            return;
                        }
                        dbConfigName = decSession.DatabaseConfigurationName;
                        explicitDbConfigName = false;
                    }
                    var repoType = data.Context.Producer.Registration.ImplementationType.GetGenericType(typeof(Repository<,>));
                    var modelType = repoType.GetGenericArguments()[0];
                    var dbConfigs = Database.GetDatabaseConfigurationsForModel(modelType);
                    if (!dbConfigs.Any())
                    {
                        throw new ActivationException($"Invalid injection of repository {data.Instance.GetType()}. " +
                                                      $"Database configuration for model {modelType} does not exist");
                    }
                    if (dbConfigs.Any(o => o.Name == dbConfigName))
                    {
                        return;
                    }
                    if (explicitDbConfigName)
                    {
                        throw new ActivationException($"Invalid injection of keyed repository {data.Instance.GetType()}. " +
                                                      $"Database configuration with the name {dbConfigName} does not contain model of type {modelType}");
                    }
                    if (dbConfigs.Count == 1)
                    {
                        repo.Session = container.GetInstance<ISession>(dbConfigs.First().Name);
                        return;
                    }
                    if(dbConfigs.All(o => o.Name != DatabaseConfiguration.DefaultName))
                    {
                        throw new ActivationException($"Invalid injection of repository {data.Instance.GetType()}. " +
                                                      $"There are multiple database configuration for model {modelType}. " +
                                                      "Hint: Use a DatabaseAttribute attribute for the injected repository to define the specific database configuration");
                    }

                }, context =>
                {
                    return Database.MultipleDatabases &&
                           context.Producer.Registration.ImplementationType.IsAssignableToGenericType(typeof(Repository<,>));
                });
            }

            // Initializer for the session factory to trigger the data population if the database recreation is set
            container.RegisterInitializer<ISessionFactory>(sessionFactory =>
            {
                var info = Database.GetSessionFactoryInfo(sessionFactory);
                if (!info.DatabaseConfiguration.RecreateAtStartup)
                {
                    return;
                }
                var eventPublisher = container.GetInstance<IEventPublisher>();
                using (var unitOfWork = container.GetInstance<IUnitOfWork>())
                {
                    try
                    {
                        eventPublisher.Publish(new PopulateDbEvent(unitOfWork));
                        unitOfWork.Commit();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// Register a fallback when a keyed repository registration is not found. Creating a conditional open generic keyed registration is not easy
        /// as it would involve a lot of reflection if we would like to mimic Simple Injector behaviuor. 
        /// The taken approach wraps the default repository resolving with manually setting the session of the keyed repository as our KeyedRegistration 
        /// class only supports simple registrations. The current limitation of this approach is that repositories must be transient as we are using the same 
        /// registration for keyed and non keyed repositories
        /// </summary>
        /// <param name="keyedDep">A Keyed dependency injection behavior</param>
        /// <param name="container">A Simple Injector container</param>
        private static void RegisterKeyedRegistrationNotFound(KeyedDependencyInjectionBehavior keyedDep, Container container)
        {
            keyedDep.OnKeyedRegistrationNotFound += (sender, args) =>
            {
                if (!typeof(IRepository).IsAssignableFrom(args.ServiceType))
                {
                    return;
                }
                var instanceProducer = container.GetRegistration(args.ServiceType);
                var implType = instanceProducer?.Registration?.ImplementationType;
                var repoType = implType?.GetGenericType(typeof(IRepository<,>));
                if (repoType == null)
                {
                    return;
                }
                var modelType = repoType.GetGenericArguments()[0];
                var name = args.Key;
                if (Database.GetDatabaseConfigurationsForModel(modelType).All(o => o.Name != name))
                {
                    throw new ActivationException(
                        $"An error occurred while injecting type {args.ServiceType} into consumer {args.Consumer}." +
                        $" There is no database configuration for model {modelType} with the name '{name}'");
                }
                if (!implType.IsAssignableToGenericType(typeof(Repository<,>)))
                {
                    throw new ActivationException("Injection of keyed repositories that do not derive from Repository<,> are not supported. " +
                                                  $"Repository type: {args.ServiceType}");
                }

                args.Register(args.ServiceType, () =>
                {
                    // Set on the current scope that we are resolvning a keyed repository so the repository 
                    // initializer will get the correct database configuration name
                    var scope = Lifestyle.Scoped.GetCurrentScope(container);
                    scope?.SetItem(ResolvingKeyedRepositoryKey, name);
                    dynamic repo = instanceProducer.GetInstance();
                    repo.Session = container.GetInstance<ISession>(name);
                    scope?.SetItem(ResolvingKeyedRepositoryKey, null);
                    return repo;
                }, name, Lifestyle.Transient);
            };
        }
    }
}
