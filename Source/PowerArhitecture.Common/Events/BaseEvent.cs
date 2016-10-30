
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Events
{
    public abstract class BaseEvent<TModel> : IEvent
    {
        protected BaseEvent(TModel message)
        {
            Message = message;
        }

        public TModel Message { get; private set; }
    }

    public abstract class BaseEvent : IEvent
    {
    }
}
