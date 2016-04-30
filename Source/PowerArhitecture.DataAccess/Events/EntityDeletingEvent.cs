using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.DataAccess.Events
{
    public class EntityDeletingEvent : BaseEvent<ISession>
    {
        public EntityDeletingEvent(ISession message) : base(message)
        {
        }
    }
}
