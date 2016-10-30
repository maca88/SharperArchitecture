using PowerArhitecture.Common.Commands;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Commands
{
    public class TestCommand : ICommand<bool>
    {
    }

    public class TestCommandHandler : BaseCommandHandler<TestCommand, bool>
    {
        public override bool Handle(TestCommand command)
        {
            return true;
        }
    }
}
