using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Event;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.DataAccess.Events
{
    public class EntitySavingEvent : BaseEvent<SaveOrUpdateEvent>
    {
        public EntitySavingEvent(SaveOrUpdateEvent message) : base(message)
        {
        }
    }
}
