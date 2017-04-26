namespace SharperArchitecture.Common.Commands
{
    public interface IAsyncCommand
    {
    }

    public interface IAsyncCommand<out TResult>
    {
    }
}
