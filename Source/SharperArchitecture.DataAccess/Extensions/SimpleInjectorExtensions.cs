using System;
using System.Linq;
using System.Reflection;
using NHibernate;
using SharperArchitecture.Common.Configuration;
using SharperArchitecture.Common.Exceptions;
using SharperArchitecture.Common.SimpleInjector;
using SharperArchitecture.DataAccess.Comparers;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Providers;
using SharperArchitecture.DataAccess.Specifications;
using SimpleInjector;

namespace SharperArchitecture.DataAccess.Extensions
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

            // Configuration
            container.RegisterDatabaseServiceSingleton(dbConfiguration, name);
            container.RegisterDatabaseServiceSingleton(dbConfiguration.NHibernateConfiguration, name);
            container.RegisterDatabaseService(() =>
            {
                return container.GetInstance<SessionFactoryProvider>().Create(name);
            }, name, Lifestyle.Singleton);

            Registration registration;

            registration = Lifestyle.Scoped.CreateRegistration(() =>
            {
                return container.GetInstance<SessionProvider>().Create(name);
            }, container);
            KeyedRegistration.Register(typeof(ISession), registration, name);

            Database.RegisteredDatabaseConfiguration(dbConfiguration);
        }

        public static bool IsOwnedByUnitOfWork(this Scope scope)
        {
            return scope?.GetItem(UnitOfWork.ScopeKey) != null;
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
                throw new SharperArchitectureException($"No database configuration found for type {modelType}.");
            }
            if (configs.Count > 1)
            {
                if (configs.Any(o => o.Name == DatabaseConfiguration.DefaultName))
                {
                    return DatabaseConfiguration.DefaultName;
                }
                throw new SharperArchitectureException($"There are multiple database configurations that contain type {modelType}.");
            }
            return configs.First().Name;
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
    }
}
