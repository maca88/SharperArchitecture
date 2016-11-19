using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IMediator _mediator;
        private static readonly Func<Mediator, ICancellableAsyncNotification, IEnumerable> GetNotificationHandlersFunc;
        private static readonly Func<object, ICancellableAsyncNotification, CancellationToken, Task> HandleFunc;

        public static bool Lambda;

        static EventPublisher()
        {
            var type = typeof(IMediator).Assembly.GetType("MediatR.Internal.CancellableAsyncNotificationHandlerWrapper");
            var handleMethod = type.GetMethod("Handle");
            var getNotificationHandlersMethod = typeof(Mediator).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(o =>
                    o.Name == "GetNotificationHandlers" && o.GetParameters().Length == 1 &&
                    o.GetParameters()[0].ParameterType == typeof(ICancellableAsyncNotification));

            var instExpr = Expression.Parameter(typeof(Mediator), "instance");
            var paramExpr = Expression.Parameter(typeof(ICancellableAsyncNotification), "message");
            var expr =  Expression.Call(instExpr, getNotificationHandlersMethod, paramExpr);
            GetNotificationHandlersFunc = Expression.Lambda<Func<Mediator, ICancellableAsyncNotification, IEnumerable>>(
                Expression.Convert(expr, typeof(IEnumerable)), instExpr, paramExpr).Compile();

            instExpr = Expression.Parameter(typeof(object), "instance");
            paramExpr = Expression.Parameter(typeof(ICancellableAsyncNotification), "message");
            var paramExpr2 = Expression.Parameter(typeof(CancellationToken), "token");
            expr = Expression.Call(Expression.Convert(instExpr, type), handleMethod, paramExpr, paramExpr2);
            HandleFunc = Expression.Lambda<Func<object, ICancellableAsyncNotification, CancellationToken, Task>>(expr, instExpr,
                    paramExpr, paramExpr2).Compile();
        }

        /// <summary>
        /// We cannot inject IMediator as it is registered as Transient because of the ICommandDispacther
        /// </summary>
        /// <param name="mediatorFactory"></param>
        public EventPublisher(Func<IMediator> mediatorFactory)
        {
            _mediator = mediatorFactory();
        }

        public virtual void Publish(IEvent e)
        {
            _mediator.Publish(e);
        }

        public virtual Task PublishAsync(IEvent e)
        {
            return PublishAsync(e, CancellationToken.None);
        }

        public virtual async Task PublishAsync(IEvent e, CancellationToken cancellationToken)
        {
            var list = GetNotificationHandlersFunc((Mediator) _mediator, e);
            foreach (var item in list)
            {
                await HandleFunc(item, e, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}
