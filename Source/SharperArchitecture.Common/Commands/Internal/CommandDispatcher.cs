using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SharperArchitecture.Common.Internal;
using SimpleInjector;

namespace SharperArchitecture.Common.Commands.Internal
{
    internal class CommandDispatcher : HandlerInvoker, ICommandDispatcher
    {
        private delegate void InvokeHandle(object obj, ICommand command);
        private delegate T InvokeHandle<out T>(object obj, object command);
        private delegate Task InvokeHandleAsync(object obj, object command, CancellationToken token);
        private delegate Task<T> InvokeHandleAsync<T>(object obj, object command, CancellationToken token);

        private readonly Container _container;

        public CommandDispatcher(Container container)
        {
            _container = container;
        }

        public virtual void Dispatch(ICommand command)
        {
            var handlerInfo = CachedHandlers.GetOrAdd(command.GetType(),
                t => new HandlerInfo(
                    typeof(ICommandHandler<>).MakeGenericType(t),
                    ht => CreateHandlerInvoker<InvokeHandle>(t, ht, "Handle")
                )
            );
            var handler = _container.GetInstance(handlerInfo.Type);
            var invoker = (InvokeHandle)handlerInfo.HandleInvoker;
            invoker(handler, command);
        }

        public TResult Dispatch<TResult>(ICommand<TResult> command)
        {
            var handlerInfo = CachedHandlers.GetOrAdd(command.GetType(),
                t => new HandlerInfo(
                    typeof(ICommandHandler<,>).MakeGenericType(t, typeof(TResult)),
                    ht => CreateHandlerInvoker<InvokeHandle<TResult>>(t, ht, "Handle")
                )
            );
            var handler = _container.GetInstance(handlerInfo.Type);
            var invoker = (InvokeHandle<TResult>)handlerInfo.HandleInvoker;
            return invoker(handler, command);
        }

        public Task DispatchAsync(IAsyncCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var handlerInfo = CachedAsyncHandlers.GetOrAdd(command.GetType(),
                t => new HandlerInfo(
                    typeof(IAsyncCommandHandler<>).MakeGenericType(t),
                    ht => CreateHandlerInvoker<InvokeHandleAsync>(t, ht, "HandleAsync",
                        Expression.Parameter(typeof(CancellationToken), "token"))
                )
            );
            var handler = _container.GetInstance(handlerInfo.Type);
            var invoker = (InvokeHandleAsync)handlerInfo.HandleInvoker;
            return invoker(handler, command, cancellationToken);
        }

        public virtual Task<TResult> DispatchAsync<TResult>(IAsyncCommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var handlerInfo = CachedAsyncHandlers.GetOrAdd(command.GetType(),
                t => new HandlerInfo(
                    typeof(IAsyncCommandHandler<,>).MakeGenericType(t, typeof(TResult)),
                    ht => CreateHandlerInvoker<InvokeHandleAsync<TResult>>(t, ht, "HandleAsync",
                        Expression.Parameter(typeof(CancellationToken), "token"))
                )
            );
            var handler = _container.GetInstance(handlerInfo.Type);
            var invoker = (InvokeHandleAsync<TResult>)handlerInfo.HandleInvoker;
            return invoker(handler, command, cancellationToken);
        }
    }
}
