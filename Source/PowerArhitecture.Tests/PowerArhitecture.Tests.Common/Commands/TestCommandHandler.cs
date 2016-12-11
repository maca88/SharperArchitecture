using PowerArhitecture.Common.Commands;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Commands
{
    public class TestCommand : ICommand<bool>
    {
    }

    public class TestCommandHandler : ICommandHandler<TestCommand, bool>
    {
        public bool Handle(TestCommand command)
        {
            return true;
        }
    }
}
