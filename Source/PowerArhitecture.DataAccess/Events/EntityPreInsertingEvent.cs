using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using NHibernate.Event;

namespace PowerArhitecture.DataAccess.Events
{
    public class EntityPreInsertingEvent : BaseEvent<PreInsertEvent>
    {
        public EntityPreInsertingEvent(PreInsertEvent message) : base(message)
        {
        }
    }
}
