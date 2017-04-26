using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Common.Events
{
    internal class DelegateEventHandler
    {
        protected DelegateEventHandler(short priority)
        {
            Priority = priority;
        }

        public short Priority { get; }
    }

    internal class DelegateEventHandler<TEvent> : DelegateEventHandler,
            IEventHandler<TEvent> where TEvent : IEvent
    {
        private readonly EventHandler<TEvent> _handler;

        public DelegateEventHandler(EventHandler<TEvent> handler, short priority) : base(priority)
        {
            _handler = handler;
        }

        public void Handle(TEvent @event)
        {
            _handler(@event);
        }
    }
}
