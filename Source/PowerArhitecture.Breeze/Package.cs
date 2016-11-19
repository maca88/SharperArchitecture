using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Breeze.Specification;
using PowerArhitecture.Common.Configuration;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace PowerArhitecture.Breeze
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<BreezeMetadataConfigurator>();
            container.Register<IBreezeRepository, BreezeRepository>(Lifestyle.Scoped);

            var modelConfigurators = AppConfiguration.GetDomainAssemblies()
                .SelectMany(o => o.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && t.IsAssignableToGenericType(typeof(IBreezeModelConfigurator)))
                .ToList();
            foreach (var modelConfigurator in modelConfigurators)
            {
                container.RegisterSingleton(modelConfigurator);
            }
            container.RegisterCollection<IBreezeModelConfigurator>(modelConfigurators);

            var interceptors = AppConfiguration.GetDomainAssemblies()
                .SelectMany(o => o.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && t.IsAssignableToGenericType(typeof(IBreezeInterceptor)))
                .ToList();
            foreach (var interceptor in interceptors)
            {
                container.RegisterSingleton(interceptor);
            }
            container.RegisterCollection<IBreezeInterceptor>(interceptors);
        }
    }
}
