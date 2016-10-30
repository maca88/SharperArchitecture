using PowerArhitecture.Common.Events;
using NHibernate;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    public class TransactionCommittedEvent : IEvent
    {
        public TransactionCommittedEvent(ISession session, bool success)
        {
            Session = session;
            Success = success;
        }

        public ISession Session { get; }

        public bool Success { get; }
    }
}
