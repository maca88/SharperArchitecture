using PowerArhitecture.Common.Events;
using NHibernate;

namespace PowerArhitecture.DataAccess.Events
{
    public class SessionDisposedEvent : BaseEvent<ISession>
    {
        public SessionDisposedEvent(ISession message) : base(message)
        {
        }
    }
}
