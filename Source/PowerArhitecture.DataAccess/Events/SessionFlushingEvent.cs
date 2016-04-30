using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.DataAccess.Events
{
    public class SessionFlushingEvent : BaseEvent<ISession>
    {
        public SessionFlushingEvent(ISession message) : base(message)
        {
        }
    }
}
