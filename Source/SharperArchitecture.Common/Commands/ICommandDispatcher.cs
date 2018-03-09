using System.Threading;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Commands
{
    public interface ICommandDispatcher
    {
        void Dispatch(ICommand command);

        TResult Dispatch<TResult>(ICommand<TResult> command);

        Task DispatchAsync(IAsyncCommand command, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> DispatchAsync<TResult>(IAsyncCommand<TResult> command, CancellationToken cancellationToken = default(CancellationToken));
    }
}
