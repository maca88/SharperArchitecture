using SharperArchitecture.Common.Events;
using SharperArchitecture.DataAccess.Specifications;
using NHibernate;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
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
