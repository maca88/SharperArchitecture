using PowerArhitecture.Common.Events;
using NHibernate;

namespace PowerArhitecture.DataAccess.Events
{
    public class TransactionCommittingEvent : BaseEvent<ISession>
    {
        public TransactionCommittingEvent(ISession message)
            : base(message)
        {
        }
    }
}
