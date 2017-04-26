

namespace SharperArchitecture.Common.Commands
{
    public interface ICommand
    {
    }

    public interface ICommand<out TResult>
    {
    }
}
