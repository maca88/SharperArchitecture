using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    public class PopulateDbEvent : IEvent
    {
        public PopulateDbEvent(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }
    }
}
