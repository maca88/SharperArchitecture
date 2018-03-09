using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SharperArchitecture.Common.Internal;
using SimpleInjector;

namespace SharperArchitecture.Common.Queries.Internal
{
    internal class DefaultQueryProcessor : HandlerInvoker, IQueryProcessor
    {
        private readonly Container _container;
        private delegate T InvokeHandle<out T>(object obj, object command);
        private delegate Task<T> InvokeHandleAsync<T>(object obj, object command, CancellationToken token);

        public DefaultQueryProcessor(Container container)
        {
            _container = container;
        }

        public TResult Process<TResult>(IQuery<TResult> query)
        {
            var handlerInfo = CachedHandlers.GetOrAdd(query.GetType(),
                t => new HandlerInfo(
                    typeof(IQueryHandler<,>).MakeGenericType(t, typeof(TResult)),
                    ht => CreateHandlerInvoker<InvokeHandle<TResult>>(t, ht, "Handle")
                )
            );
            var handler = _container.GetInstance(handlerInfo.Type);
            var invoker = (InvokeHandle<TResult>)handlerInfo.HandleInvoker;
            return invoker(handler, query);
        }

        public Task<TResult> ProcessAsync<TResult>(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var handlerInfo = CachedAsyncHandlers.GetOrAdd(query.GetType(),
                t => new HandlerInfo(
                    typeof(IAsyncQueryHandler<,>).MakeGenericType(t, typeof(TResult)),
                    ht => CreateHandlerInvoker<InvokeHandleAsync<TResult>>(t, ht, "HandleAsync",
                        Expression.Parameter(typeof(CancellationToken), "token"))
                )
            );
            var handler = _container.GetInstance(handlerInfo.Type);
            var invoker = (InvokeHandleAsync<TResult>)handlerInfo.HandleInvoker;
            return invoker(handler, query, cancellationToken);
        }
    }
}
