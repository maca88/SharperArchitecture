using PowerArhitecture.Common.Events;
using NHibernate;

namespace PowerArhitecture.DataAccess.Events
{
    public class TransactionBeganEvent : BaseEvent<ISession>
    {
        public TransactionBeganEvent(ISession message)
            : base(message)
        {
        }
    }
}
