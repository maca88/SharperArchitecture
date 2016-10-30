using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    public class PopulateDbEvent : IEvent
    {
        public PopulateDbEvent(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; }
    }
}
