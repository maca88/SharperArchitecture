using SharperArchitecture.Common.Commands;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Tests.Common.Commands
{
    public class VoidCommand : ICommand
    {
        public bool Success { get; set; }
    }

    public class VoidCommandHandler : ICommandHandler<VoidCommand>
    {
        public void Handle(VoidCommand command)
        {
            command.Success = true;
        }
    }
}
