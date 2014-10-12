using PowerArhitecture.Common.Events;
using NHibernate;

namespace PowerArhitecture.DataAccess.Events
{
    public class SessionDisposingEvent : BaseEvent<ISession>
    {
        public SessionDisposingEvent(ISession message) : base(message)
        {
        }
    }
}
