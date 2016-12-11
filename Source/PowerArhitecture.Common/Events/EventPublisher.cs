using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Specifications;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace PowerArhitecture.Common.Events
{
    public class EventPublisher : IEventPublisher
    {
        private delegate void InvokeHandle(object obj, IEvent @event);
        private delegate Task InvokeHandleAsync(object obj, IAsyncEvent @event, CancellationToken token);

        private readonly Container _container;
        static readonly ConcurrentDictionary<Type, object> EventHandlerInvokers = new ConcurrentDictionary<Type, object>();

        public EventPublisher(Container container)
        {
            _container = container;
        }

        public virtual void Publish(IEvent e)
        {
            var eventType = e.GetType();
            var handlerType = typeof(IEventHandler<>).MakeGenericType(e.GetType());
            var handle = (InvokeHandle)EventHandlerInvokers.GetOrAdd(handlerType, t =>
                CreateHandlerInvoker<InvokeHandle>(
                    eventType, handlerType, "Handle"));
            foreach (var obj in GetInstances(handlerType))
            {
                handle(obj, e);
            }
        }

        public virtual Task PublishAsync(IAsyncEvent e)
        {
            return PublishAsync(e, CancellationToken.None);
        }

        public virtual async Task PublishAsync(IAsyncEvent e, CancellationToken cancellationToken)
        {
            var eventType = e.GetType();
            var handlerType = typeof(IAsyncEventHandler<>).MakeGenericType(e.GetType());
            var handleAsync = (InvokeHandleAsync)EventHandlerInvokers.GetOrAdd(handlerType, t =>
                CreateHandlerInvoker<InvokeHandleAsync>(
                    eventType, handlerType, "HandleAsync",
                    Expression.Parameter(typeof(CancellationToken), "token")));
            foreach (var obj in GetInstances(handlerType))
            {
                await handleAsync(obj, e, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private IEnumerable<object> GetInstances(Type t)
        {
            return _container.TryGetAllInstances(t).OrderByDescending(o =>
            {
                return o.GetType().GetCustomAttribute<PriorityAttribute>()?.Priority ??
                       PriorityAttribute.Default;
            });
        }

        private static TResult CreateHandlerInvoker<TResult>(Type eventType, Type handlerType, string methodName, params ParameterExpression[] parameters)
        {
            var param1 = Expression.Parameter(typeof(object), "handler");
            var param2 = Expression.Parameter(typeof(object), "obj");
            var convertHandler = Expression.Convert(param1, handlerType);
            var convertCommand = Expression.Convert(param2, eventType);
            var callParams = new List<Expression>
            {
                convertCommand
            }.Concat(parameters);
            var lambdaParams = new List<ParameterExpression>
            {
                param1,
                param2
            }.Concat(parameters);
            var callMethod = Expression.Call(convertHandler, handlerType.GetMethod(methodName), callParams);
            return Expression.Lambda<TResult>(callMethod, lambdaParams).Compile();
        }
    }
}
