using System;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Providers;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace PowerArhitecture.DataAccess.Extensions
{
    public static class SimpleInjectorExtensions
    {
        public static void RegisterDatabaseConfiguration(this Container container, DatabaseConfiguration dbConfiguration)
        {
            var name = dbConfiguration.Name;

            container.RegisterDatabaseServiceSingleton(dbConfiguration, name);
            container.RegisterDatabaseServiceSingleton(dbConfiguration.NHibernateConfiguration, name);
            container.RegisterDatabaseService(() =>
            {
                return container.GetInstance<SessionFactoryProvider>().Create(name);
            }, name, Lifestyle.Singleton);
            container.RegisterDatabaseService(() =>
            {
                return container.GetInstance<SessionProvider>().Create(name);
            }, name, Lifestyle.Scoped);

            Database.RegisteredDatabaseConfigurations.AddOrUpdate(name, dbConfiguration, (k, v) => dbConfiguration);
        }

        private static void RegisterDatabaseServiceSingleton<TService>(this Container container, TService instance, string dbConfigName) where TService : class
        {
            if (dbConfigName == DatabaseConfiguration.DefaultName)
            {
                container.RegisterSingleton(instance);
            }
            else
            {
                container.RegisterSingleton(instance, dbConfigName);
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
                container.Register(instanceCreator, dbConfigName, lifestyle);
            }
        }

        internal static TService GetDatabaseService<TService>(this Container container, string dbConfigName) where TService : class
        {
            return DatabaseConfiguration.DefaultName == dbConfigName 
                ? container.GetInstance<TService>() 
                : container.GetInstance<TService>(dbConfigName);
        }
    }
}
