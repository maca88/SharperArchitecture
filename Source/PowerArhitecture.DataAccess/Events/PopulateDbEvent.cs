using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;

namespace PowerArhitecture.DataAccess.Events
{
    public class PopulateDbEvent : BaseEvent<IUnitOfWork>
    {
        public PopulateDbEvent(IUnitOfWork message)
            : base(message)
        {
        }
    }
}
