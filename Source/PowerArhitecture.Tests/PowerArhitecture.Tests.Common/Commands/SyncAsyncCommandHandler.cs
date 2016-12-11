using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Commands
{
    public class SyncAsyncCommand : ICommand<bool>, IAsyncCommand<bool>
    {
        public int Counter { get; set; }
    }

    public class SyncAsyncCommandHandler : 
        IAsyncCommandHandler<SyncAsyncCommand, bool>,
        ICommandHandler<SyncAsyncCommand, bool>
    {
        public async Task<bool> HandleAsync(SyncAsyncCommand command, CancellationToken cancellationToken)
        {
            await Task.Yield();
            command.Counter++;
            return true;
        }

        public bool Handle(SyncAsyncCommand command)
        {
            command.Counter++;
            return true;
        }
    }
}
