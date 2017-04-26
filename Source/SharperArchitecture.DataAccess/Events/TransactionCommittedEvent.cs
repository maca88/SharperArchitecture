using SharperArchitecture.Common.Events;
using NHibernate;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
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
