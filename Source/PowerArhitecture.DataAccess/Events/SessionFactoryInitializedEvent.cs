using PowerArhitecture.Common.Events;
using NHibernate;

namespace PowerArhitecture.DataAccess.Events
{
    public class SessionFactoryInitializedEvent : BaseEvent<ISessionFactory>
    {
        public SessionFactoryInitializedEvent(ISessionFactory message) : base(message)
        {
        }
    }
}
