using PowerArhitecture.Common.Commands;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Commands
{
    public class VoidCommand : ICommand
    {
        public bool Success { get; set; }
    }

    public class VoidCommandHandler : BaseCommandHandler<VoidCommand>
    {
        public override void Handle(VoidCommand command)
        {
            command.Success = true;
        }
    }
}
