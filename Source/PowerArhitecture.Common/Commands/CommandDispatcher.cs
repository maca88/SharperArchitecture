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
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IMediator _mediator;

        public CommandDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public virtual void Dispatch(ICommand command)
        {
            _mediator.Send(command);
        }

        public TResult Dispatch<TResult>(ICommand<TResult> command)
        {
            return _mediator.Send(command);
        }

        public virtual Task DispatchAsync(ICommand command)
        {
            return _mediator.SendAsync(command, CancellationToken.None);
        }

        public Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command)
        {
            return _mediator.SendAsync(command, CancellationToken.None);
        }

        public Task DispatchAsync(ICommand command, CancellationToken cancellationToken)
        {
            return _mediator.SendAsync(command, cancellationToken);
        }

        public virtual Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        {
            return _mediator.SendAsync(command, cancellationToken);
        }
    }
}
