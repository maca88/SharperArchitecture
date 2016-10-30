using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Commands
{
    public abstract class BaseCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public abstract void Handle(TCommand message);

        public virtual Task HandleAsync(TCommand message, CancellationToken cancellationToken)
        {
            try
            {
                Handle(message);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        Unit IRequestHandler<TCommand, Unit>.Handle(TCommand message)
        {
            Handle(message);
            return Unit.Value;
        }

        async Task<Unit> ICancellableAsyncRequestHandler<TCommand, Unit>.Handle(TCommand message, CancellationToken cancellationToken)
        {
            await HandleAsync(message, cancellationToken);
            return Unit.Value;
        }
    }

    public abstract class BaseCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        public abstract TResult Handle(TCommand message);

        public virtual Task<TResult> HandleAsync(TCommand message, CancellationToken cancellationToken)
        {
            try
            {
                return Task.FromResult(Handle(message));
            }
            catch (Exception e)
            {
                return Task.FromException<TResult>(e);
            }
        }

        Task<TResult> ICancellableAsyncRequestHandler<TCommand, TResult>.Handle(TCommand message, CancellationToken cancellationToken)
        {
            return HandleAsync(message, cancellationToken);
        }
    }
}
