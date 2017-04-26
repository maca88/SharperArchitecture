using SharperArchitecture.Common.Events;
using NHibernate;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
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
