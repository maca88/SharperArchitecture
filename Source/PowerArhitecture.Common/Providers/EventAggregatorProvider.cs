using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Publishers;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Logging;
using Ninject.Syntax;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.Common.Providers
{
    public class EventAggregatorProvider : IProvider<IEventAggregator>
    {
        public object Create(IContext context)
        {
            var eventAggregator = new EventAggregator();
            return eventAggregator;
        }

        public static void OnActivation(IContext context, IEventAggregator aggregator)
        {
            foreach (var listenerType in AppConfiguration.GetDomainAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(o => !o.IsInterface && !o.IsAbstract && o.IsAssignableToGenericType(typeof(IListener<>))
                        && o != typeof(DelegateListener<>) /*&& o != typeof(DelegateListenerAsync<>)*/))
                    .OrderByDescending(o =>
                    {
                        var attr = o.GetCustomAttribute<PriorityAttribute>();
                        return attr?.Priority ?? 0;
                    }))
            {
                //Bind all listener that are not already binded
                if (!context.Kernel.GetBindings(listenerType).Any())
                    context.Kernel.Bind(listenerType).ToSelf().InSingletonScope(); //bind to singleton so that GC will not remove it

                //_logger.Info("Adding new listener '{0}'", listenerType.FullName);
                aggregator.AddListener(context.Kernel.Get(listenerType));
            }
            new UnhandledExceptionPublisher(aggregator); //TODO: move to another place
        }

        public Type Type => typeof(EventAggregator);
    }
}
