using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Event;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
{
    public class SessionFactoryCreatedEvent : IEvent
    {
        public SessionFactoryCreatedEvent(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        public ISessionFactory SessionFactory { get; }
    }
}
