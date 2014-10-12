using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Publishers;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Logging;
using Ninject.Syntax;

namespace PowerArhitecture.Common.Providers
{
    public class EventAggregatorProvider : IProvider<IEventAggregator>
    {
        public object Create(IContext context)
        {
            var eventAggregator = new EventAggregator();
            Type = eventAggregator.GetType();
            return eventAggregator;
        }

        public static void OnActivation(IContext context, IEventAggregator aggregator)
        {
            foreach (var listenerType in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(o => !o.IsInterface && !o.IsAbstract && o.IsAssignableToGenericType(typeof(IListener<>)) && o != typeof(DelegateListener<>))))
            {
                //Bind all listener that are not already binded
                if (!context.Kernel.GetBindings(listenerType).Any())
                    context.Kernel.Bind(listenerType).ToSelf().InSingletonScope(); //bind to singleton so that GC will not remove it

                //_logger.Info("Adding new listener '{0}'", listenerType.FullName);
                aggregator.AddListener(context.Kernel.Get(listenerType));
            }
            new UnhandledExceptionPublisher(aggregator); //TODO: move to another place
        }

        public Type Type { get; private set; }
    }
}
