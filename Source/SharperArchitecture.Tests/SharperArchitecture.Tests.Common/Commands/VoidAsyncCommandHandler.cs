using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Tests.Common.Commands
{
    public class VoidAsyncCommand : IAsyncCommand
    {
        public bool Success { get; set; }
    }

    public class VoidAsyncCommandHandler : IAsyncCommandHandler<VoidAsyncCommand>
    {
        public async Task HandleAsync(VoidAsyncCommand command, CancellationToken cancellationToken)
        {
            await Task.Yield();
            command.Success = true;
        }
    }
}
