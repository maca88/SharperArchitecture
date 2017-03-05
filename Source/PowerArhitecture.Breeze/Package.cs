using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider.NH;
using PowerArhitecture.Breeze.Configuration;
using PowerArhitecture.Breeze.Specification;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using SimpleInjector.Packaging;

namespace PowerArhitecture.Breeze
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton(() =>
            {
                return new NHibernateContractResolver(type =>
                {
                    var dbConfig = Database.GetDatabaseConfigurationsForModel(type).SingleOrDefault();
                    return dbConfig == null
                        ? null
                        : container.GetInstance<ISessionFactoryProvider>().Get(dbConfig.Name).GetClassMetadata(type);
                }, container.GetInstance<IBreezeConfigurator>());
            });

            container.RegisterSingleton<IBreezeConfigurator, BreezeConfigurator>();
            container.RegisterSingleton<IBreezeConfiguration>(new BreezeConfiguration());

            container.Register<IBreezeContext, BreezeContext>(Lifestyle.Scoped);
        }
    }
}
