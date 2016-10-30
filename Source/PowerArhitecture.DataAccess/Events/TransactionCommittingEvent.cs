using PowerArhitecture.Common.Events;
using NHibernate;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    public class TransactionCommittingEvent : IEvent
    {
        public TransactionCommittingEvent(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }
    }
}
