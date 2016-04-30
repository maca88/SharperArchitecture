using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Event;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.DataAccess.Events
{
    public class SessionFactoryCreatedEvent : BaseEvent<ISessionFactory>
    {
        public SessionFactoryCreatedEvent(ISessionFactory message) : base(message)
        {
        }
    }
}
