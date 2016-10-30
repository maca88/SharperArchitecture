
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Events
{
    public abstract class MessageEvent<TMessage> : IEvent
    {
        protected MessageEvent(TMessage message)
        {
            Message = message;
        }

        public TMessage Message { get; private set; }
    }

}
