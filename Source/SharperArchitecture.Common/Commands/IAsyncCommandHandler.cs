using System.Threading;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Commands
{
    public interface IAsyncCommandHandler<in TCommand> where TCommand : IAsyncCommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IAsyncCommandHandler<in TCommand, TResult> where TCommand : IAsyncCommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default(CancellationToken));
    }
}
