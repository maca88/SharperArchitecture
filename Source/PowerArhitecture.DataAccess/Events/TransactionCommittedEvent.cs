using PowerArhitecture.Common.Events;
using NHibernate;

namespace PowerArhitecture.DataAccess.Events
{
    public class TransactionCommittedEvent : BaseEvent<ISession>
    {
        public TransactionCommittedEvent(ISession message) : base(message)
        {
        }
    }
}
