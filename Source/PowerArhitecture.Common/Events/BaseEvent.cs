
namespace PowerArhitecture.Common.Events
{
    public class BaseEvent<TModel>
    {
        public BaseEvent(TModel message)
        {
            Message = message;
        }

        public TModel Message { get; private set; }
    }
}
