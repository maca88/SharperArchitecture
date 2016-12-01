using System;
using System.Linq;
using System.Reflection;
using NHibernate;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Common.SimpleInjector;
using PowerArhitecture.DataAccess.Comparers;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Providers;
using PowerArhitecture.DataAccess.Specifications;
using SimpleInjector;

namespace PowerArhitecture.DataAccess.Extensions
{
    public static class SimpleInjectorExtensions
    {
        internal static KeyedRegistration KeyedRegistration;

        public static void RegisterDatabaseConfiguration(this Container container, DatabaseConfiguration dbConfiguration)
        {
            var name = dbConfiguration.Name;

            if (KeyedRegistration == null)
            {
                KeyedRegistration = new KeyedRegistration(container);
            }

            // Throw if the default session was already registered
            if (container.GetCurrentRegistrations()
                .Any(o => o.ServiceType == typeof(ISession) && o.GetPredicate() == NotHandledPredicate))
            {
                throw new PowerArhitectureException("container.RegisterDatabaseConfiguration must be called before container.RegisterPackages");
            }

            // Configuration
            container.RegisterDatabaseServiceSingleton(dbConfiguration, name);
            container.RegisterDatabaseServiceSingleton(dbConfiguration.NHibernateConfiguration, name);
            container.RegisterDatabaseService(() =>
            {
                return container.GetInstance<SessionFactoryProvider>().Create(name);
            }, name, Lifestyle.Singleton);

            Registration registration;

            var currRegisteredServices = container.GetCurrentRegistrations().Select(o => o.ServiceType).ToHashSet();

            if (name == DatabaseConfiguration.DefaultName)
            {
                // Ignore all found candidates for custom repositories
                var customRepoCandidates = AppConfiguration.GetDomainAssemblies()
                    .SelectMany(o => o.GetTypes())
                    .Where(t =>
                        t.IsInterface &&
                        t.IsGenericType &&
                        t.GetGenericArguments().Length == 2 &&
                        t != typeof(IRepository<,>) &&
                        t.IsAssignableToGenericType(typeof(IRepository<,>)))
                    .ToHashSet();

                AppConfiguration.GetDomainAssemblies()
                    .SelectMany(o => o.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && typeof(IRepository).IsAssignableFrom(t))
                    .Where(t => t.GetInterfaces().All(o => customRepoCandidates.All(c => !o.IsAssignableToGenericType(c))))
                    .Select(t => new {
                        Implementation = t,
                        Services = t.GetInterfaces()
                            .Where(o => o.IsAssignableToGenericType(typeof(IRepository<,>)))
                    })
                    .ForEach(o =>
                    {
                        registration = Lifestyle.Scoped.CreateRegistration(o.Implementation, container);
                        foreach (var serviceType in o.Services.Where(t => !currRegisteredServices.Contains(t)))
                        {
                            container.AddRegistration(serviceType, registration);
                        }
                    });
                    container.RegisterConditional(typeof(IRepository<>), typeof(Repository<>), Lifestyle.Scoped, NotHandledPredicate);
                    container.RegisterConditional(typeof(IRepository<,>), typeof(Repository<,>), Lifestyle.Scoped, NotHandledPredicate);

                Database.RegisteredDatabaseConfiguration(dbConfiguration);
                return;
            }

            Type baseRepoType = null;

            if (dbConfiguration.RepositoryTypes.Any())
            {
                baseRepoType = dbConfiguration.RepositoryTypes.Keys.OrderBy(o => o, new TypeInheritanceComparer()).First();

                AppConfiguration.GetDomainAssemblies()
                    .SelectMany(o => o.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && t.IsAssignableToGenericType(baseRepoType))
                    .Select(t => new {
                        Implementation = t,
                        Services = t.GetInterfaces().Where(o => o.IsAssignableToGenericType(baseRepoType))
                    })
                    .ForEach(o =>
                    {
                        registration = Lifestyle.Scoped.CreateRegistration(o.Implementation, container);
                        foreach (var serviceType in o.Services.Where(t => !currRegisteredServices.Contains(t)))
                        {
                            container.AddRegistration(serviceType, registration);
                        }
                    });

                foreach (var pair in dbConfiguration.RepositoryTypes)
                {
                    container.RegisterConditional(pair.Key, pair.Value, Lifestyle.Scoped, NotHandledPredicate);
                }
            }

            registration = Lifestyle.Scoped.CreateRegistration(() =>
            {
                return container.GetInstance<SessionProvider>().Create(name);
            }, container);
            KeyedRegistration.Register(typeof(ISession), registration, name);

            container.RegisterConditional(typeof(ISession), registration, context =>
            {
                var defRepoType = context.Consumer?.ServiceType?.GetGenericType(typeof(IRepository<,>));
                if (defRepoType == null)
                {
                    return false;
                }
                var modelType = defRepoType.GetGenericArguments()[0];
                var repoType = baseRepoType == null ? null : context.Consumer?.ServiceType?.GetGenericType(baseRepoType);
                var configs = Database.GetDatabaseConfigurationsForModel(modelType);
                if (!configs.Any())
                {
                    throw new ActivationException($"An error occurred while injecting type {context.ServiceType} into consumer {context.Consumer}. " +
                                                  $"Database configuration for model {modelType} does not exist");
                }
                if (repoType != null && configs.Any(o => o.Name == name))
                {
                    return true;
                }
                if (repoType == null && configs.Any(o => o.Name == DatabaseConfiguration.DefaultName))
                {
                    return false;
                }
                if (repoType != null)
                {
                    throw new ActivationException(
                        $"An error occurred while injecting type {context.ServiceType} into consumer {context.Consumer}. " +
                        $"There is no database configuration for model {modelType} with the name '{name}'");
                }
                if (configs.Count > 1)
                {
                    throw new ActivationException(
                        $"An error occurred while injecting type {context.ServiceType} into consumer {context.Consumer}. " +
                        $"There are multiple database configuration for model {modelType}. " +
                        "Hint: use a registered repository interface instead of the default one. Example: IBarRepository<Model>");
                }
                return configs.Any(o => o.Name == name);
            });

            Database.RegisteredDatabaseConfiguration(dbConfiguration);
        }

        public static bool IsOwnedByUnitOfWork(this Scope scope)
        {
            return scope?.GetItem(UnitOfWork.ScopeKey) != null;
        }

        internal static bool NotHandledPredicate(PredicateContext context)
        {
            return !context.Handled;
        }

        internal static string GetDatabaseConfigurationNameForModel(this Type modelType)
        {
            if (!Database.MultipleDatabases && Database.HasDefaultDatabase)
            {
                return DatabaseConfiguration.DefaultName;
            }
            var configs = Database.GetDatabaseConfigurationsForModel(modelType).ToList();
            if (!configs.Any())
            {
                throw new PowerArhitectureException($"No database configuration found for type {modelType}.");
            }
            if (configs.Count > 1)
            {
                if (configs.Any(o => o.Name == DatabaseConfiguration.DefaultName))
                {
                    return DatabaseConfiguration.DefaultName;
                }
                throw new PowerArhitectureException($"There are multiple database configurations that contain type {modelType}.");
            }
            return configs.First().Name;
        }

        internal static string GetDatabaseConfigurationNameForRepository(this Type repoType)
        {
            var defRepoType = repoType.GetGenericType(typeof(IRepository<,>));
            if (defRepoType == null)
            {
                throw new ActivationException("Repository must be assignable to interface IRepository<,>");
            }
            if (!Database.MultipleDatabases && Database.HasDefaultDatabase)
            {
                return DatabaseConfiguration.DefaultName;
            }
            var modelType = defRepoType.GetGenericArguments()[0];
            var configs = Database.GetDatabaseConfigurationsForModel(modelType);
            Type customRepoType = null;
            var dbConfigName = DatabaseConfiguration.DefaultName;
            foreach (var dbConfig in Database.GetRegisteredDatabaseConfigurations().Where(o => o.RepositoryTypes.Any()))
            {
                var baseRepoType = dbConfig.RepositoryTypes.Keys.OrderBy(o => o, new TypeInheritanceComparer()).First();
                customRepoType = repoType.GetGenericType(baseRepoType);
                if (customRepoType != null)
                {
                    dbConfigName = dbConfig.Name;
                    break;
                }
            }
            if (customRepoType != null && configs.Any(o => o.Name == dbConfigName))
            {
                return dbConfigName;
            }
            if (customRepoType == null && configs.Any(o => o.Name == DatabaseConfiguration.DefaultName))
            {
                return DatabaseConfiguration.DefaultName;
            }
            if (customRepoType != null || configs.All(o => o.Name != dbConfigName))
            {
                throw new ActivationException(
                    $"An error occurred while activating repository {repoType}. " +
                    $"There is no database configuration for model {modelType} with the name '{dbConfigName}'");
            }
            if (configs.Count > 1)
            {
                throw new ActivationException(
                    $"An error occurred while activating repository {repoType}. " +
                    $"There are multiple database configuration for model {modelType}. " +
                    "Hint: use a registered repository interface instead of the default one. Example: IBarRepository<Model>");
            }
            return dbConfigName;
        }

        internal static TService GetDatabaseService<TService>(this Container container, string dbConfigName) where TService : class
        {
            return DatabaseConfiguration.DefaultName == dbConfigName
                ? container.GetInstance<TService>()
                : (TService)KeyedRegistration.GetRegistration(typeof(TService), dbConfigName).GetInstance();
        }

        private static void RegisterDatabaseServiceSingleton<TService>(this Container container, TService instance, string dbConfigName) where TService : class
        {
            if (dbConfigName == DatabaseConfiguration.DefaultName)
            {
                container.RegisterSingleton(instance);
            }
            else
            {
                KeyedRegistration.Register(typeof(TService), () => instance, dbConfigName, Lifestyle.Singleton);
            }
        }

        private static void RegisterDatabaseService<TService>(this Container container, Func<TService> instanceCreator, string dbConfigName, Lifestyle lifestyle)
            where TService : class
        {
            if (dbConfigName == DatabaseConfiguration.DefaultName)
            {
                container.Register(instanceCreator, lifestyle);
            }
            else
            {
                KeyedRegistration.Register(typeof(TService), instanceCreator, dbConfigName, lifestyle);
            }
        }

        private static Predicate<PredicateContext> GetPredicate(this InstanceProducer producer)
        {
            return producer.GetMemberValue("Predicate") as Predicate<PredicateContext>;
        }
    }
}
