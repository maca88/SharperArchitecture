

namespace SharperArchitecture.Common.Specifications
{
    public interface ICommand
    {
    }

    public interface ICommand<out TResult>
    {
    }
}
