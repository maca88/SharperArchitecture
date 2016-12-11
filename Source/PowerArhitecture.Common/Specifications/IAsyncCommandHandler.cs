using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Specifications
{
    public interface IAsyncCommandHandler<in TCommand> where TCommand : IAsyncCommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }

    public interface IAsyncCommandHandler<in TCommand, TResult> where TCommand : IAsyncCommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
