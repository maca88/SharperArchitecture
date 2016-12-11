

namespace PowerArhitecture.Common.Specifications
{
    public interface IEventHandler<in TEvent>  where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}
