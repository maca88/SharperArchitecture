using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Specifications
{
    public interface ICommandDispatcher
    {
        void Dispatch(ICommand command);

        TResult Dispatch<TResult>(ICommand<TResult> command);

        Task DispatchAsync(ICommand command);

        Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command);

        Task DispatchAsync(ICommand command, CancellationToken cancellationToken);

        Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken);
    }
}
