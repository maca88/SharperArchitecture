using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SharperArchitecture.Common.Exceptions;
using SharperArchitecture.WebApi;
using SharperArchitecture.WebApi.Internal;
using SimpleInjector.Advanced;

namespace SimpleInjector
{
    public static class SimpleInjectorExtensions
    {
        private static readonly ConcurrentDictionary<Registration, HttpFilterInfo> RegistrationScopes =
            new ConcurrentDictionary<Registration, HttpFilterInfo>();

        static SimpleInjectorExtensions() { }

        public static void RegisterHttpConfiguration(this Container container, HttpConfiguration configuration)
        {
            RegisterHttpConfiguration<TransactionAttributeProvider>(container, configuration);
        }

        public static void RegisterHttpConfiguration<TTransactionProvider>(this Container container, HttpConfiguration configuration)
            where TTransactionProvider : class, ITransactionAttributeProvider
        {
            container.RegisterWebApiControllers(configuration);
            container.EnableHttpRequestMessageTracking(configuration);

            container.RegisterSingleton<ITransactionAttributeProvider, TTransactionProvider>();
            configuration.MessageHandlers.Insert(0, new DelegatingHandlerProxy<TransactionManager>(container));

            var defaultprovider = configuration.Services.GetFilterProviders().First(p => p is ActionDescriptorFilterProvider);
            configuration.Services.Remove(typeof(IFilterProvider), defaultprovider);

            configuration.Services.Add(typeof(IFilterProvider), new OrderedFilterProvider(container));
        }

        public static void RegisterHttpFilter<TFilter>(this Container container, FilterScope scope)
            where TFilter : class, IFilter
        {
            var registration = Lifestyle.Singleton.CreateRegistration<TFilter>(container);
            container.AppendToCollection(typeof(IFilter), registration);
            var info = new HttpFilterInfo(scope);
            RegistrationScopes.AddOrUpdate(registration, info, (k, v) => info);
        }

        internal static void RegisterHttpFilter(this Container container, FilterScope scope, Registration registration)
        {
            container.AppendToCollection(typeof(IFilter), registration);
            var info = new HttpFilterInfo(scope);
            RegistrationScopes.AddOrUpdate(registration, info, (k, v) => info);
        }

        public static void RegisterHttpFilterConditional<TFilter>(this Container container, FilterScope scope,
            Predicate<HttpActionDescriptor> predicate) where TFilter : class, IFilter
        {
            var registration = Lifestyle.Singleton.CreateRegistration<TFilter>(container);
            container.AppendToCollection(typeof(IFilter), registration);
            var info = new HttpFilterInfo(scope);
            RegistrationScopes.AddOrUpdate(registration, info, (k, v) => info);
        }

        internal static HttpFilterInfo GetHttpFilterInfo<TFilter>(this Container container, TFilter filter)
            where TFilter : class, IFilter
        {
            var type = filter.GetType();
            var producer =
                container.GetCurrentRegistrations().FirstOrDefault(o => o.Registration.ImplementationType == type);
            if (producer == null)
            {
                throw new SharperArchitectureException($"Unable to get the FilterScope for the filter {type} as it is not registered. " +
                                                    "Hint: Register the filter using the method: container.RegisterHttpFilter");
            }
            HttpFilterInfo info;
            if (!RegistrationScopes.TryGetValue(producer.Registration, out info))
            {
                throw new SharperArchitectureException($"Unable to get the FilterScope for the filter {type} as it is not registered. " +
                                                    "Hint: Register the filter using the method: container.RegisterHttpFilter");
            }
            return info;
        }
    }
}
