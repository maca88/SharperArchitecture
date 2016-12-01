using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;
using MediatR;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.WebApi.Internal;
using PowerArhitecture.WebApi.Specifications;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Packaging;

namespace PowerArhitecture.WebApi
{
    /// <summary>
    /// Registration point where all services are registered.
    /// This package must be executed before PowerArhitecture.Common package, otherwise registration of TransactionManager will break
    /// </summary>
    [Priority(10)]
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            var requestMessageProvider = new RequestMessageProvider(container);
            container.RegisterSingleton<IRequestMessageProvider>(requestMessageProvider);

            container.RegisterCollection<IFilter>();

            var registration = Lifestyle.Singleton.CreateRegistration<TransactionManager>(container);
            container.AddRegistration(typeof(TransactionManager), registration);
            container.AppendToCollection(typeof(INotificationHandler<SessionCreatedEvent>), registration);
            container.RegisterHttpFilter(FilterScope.Action, registration);
        }
    }
}
