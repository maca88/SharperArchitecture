using PowerArhitecture.Common.Events;
using NHibernate;

namespace PowerArhitecture.DataAccess.Events
{
    public class SessionCreatedEvent : BaseEvent<ISession>
    {
        public SessionCreatedEvent(ISession message) : base(message)
        {
        }
    }
}
