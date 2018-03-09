using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SharperArchitecture.Common.Internal;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace SharperArchitecture.Common.Events.Internal
{
    public delegate void EventHandler<in TEvent>(TEvent @event) where TEvent : IEvent;
    public delegate Task AsyncEventHandler<in TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IAsyncEvent;

    internal class EventAggregator : HandlerInvoker, IEventPublisher, IEventSubscriber
    {
        private delegate void InvokeHandle(object obj, IEvent @event);
        private delegate Task InvokeHandleAsync(object obj, IAsyncEvent @event, CancellationToken token);

        private readonly Container _container;
        private readonly ConcurrentDictionary<object, DelegateEventHandler> _delegateEventHandlers = 
            new ConcurrentDictionary<object, DelegateEventHandler>();

        public EventAggregator(Container container)
        {
            _container = container;
        }

        public virtual void Publish(IEvent e)
        {
            var handlerInfo = CachedHandlers.GetOrAdd(e.GetType(),
                t => new HandlerInfo(
                    typeof(IEventHandler<>).MakeGenericType(t),
                    ht => CreateHandlerInvoker<InvokeHandle>(t, ht, "Handle")
                )
            );
            var invoker = (InvokeHandle)handlerInfo.HandleInvoker;
            foreach (var obj in GetInstances(handlerInfo.Type))
            {
                invoker(obj, e);
            }
        }

        public virtual async Task PublishAsync(IAsyncEvent e, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventType = e.GetType();
            var handlerInfo = CachedAsyncHandlers.GetOrAdd(eventType,
                t => new HandlerInfo(
                    typeof(IAsyncEventHandler<>).MakeGenericType(t),
                    ht => CreateHandlerInvoker<InvokeHandleAsync>(t, ht, "HandleAsync",
                        Expression.Parameter(typeof(CancellationToken), "token"))
                )
            );
            var invoker = (InvokeHandleAsync)handlerInfo.HandleInvoker;
            foreach (var obj in GetInstances(handlerInfo.Type))
            {
                await invoker(obj, e, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        public void Subscribe<TEvent>(EventHandler<TEvent> handlerDelegate, short priority = default(short)) where TEvent : IEvent
        {
            var handler = new DelegateEventHandler<TEvent>(handlerDelegate, priority);
            _delegateEventHandlers.AddOrUpdate(handlerDelegate, handler, (k, v) => handler);
        }

        public bool Unsubscribe<TEvent>(EventHandler<TEvent> handlerDelegate) where TEvent : IEvent
        {
            return _delegateEventHandlers.TryRemove(handlerDelegate, out _);
        }

        public void Subscribe<TEvent>(AsyncEventHandler<TEvent> handlerDelegate, short priority = default(short)) where TEvent : IAsyncEvent
        {
            var handler = new AsyncDelegateEventHandler<TEvent>(handlerDelegate, priority);
            _delegateEventHandlers.AddOrUpdate(handlerDelegate, handler, (k, v) => handler);
        }

        public bool Unsubscribe<TEvent>(AsyncEventHandler<TEvent> handlerDelegate) where TEvent : IAsyncEvent
        {
            return _delegateEventHandlers.TryRemove(handlerDelegate, out _);
        }

        private IEnumerable<object> GetInstances(Type t)
        {
            return _container
                .TryGetAllInstances(t)
                .Select(o => new
                {
                    Instance = o,
                    Priority = o.GetType().GetPriority()
                })
                .Union(
                    _delegateEventHandlers.Values
                    .Where(t.IsInstanceOfType)
                    .Select(o => new
                    {
                        Instance = (object)o,
                        o.Priority
                    })
                )
                .OrderByDescending(o => o.Priority)
                .Select(o => o.Instance);
        }
    }
}
