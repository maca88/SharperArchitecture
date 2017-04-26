using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Events;
using SharperArchitecture.WebApi.Internal;
using SharperArchitecture.WebApi.Specifications;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Packaging;

namespace SharperArchitecture.WebApi
{
    /// <summary>
    /// Registration point where all services are registered.
    /// This package must be executed before SharperArchitecture.Common package, otherwise registration of TransactionManager will break
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
            container.AppendToCollection(typeof(IEventHandler<SessionCreatedEvent>), registration);
            container.RegisterHttpFilter(FilterScope.Action, registration);
        }
    }
}
