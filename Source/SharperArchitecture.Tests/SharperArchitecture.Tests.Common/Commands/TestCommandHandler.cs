using SharperArchitecture.Common.Commands;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Tests.Common.Commands
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
