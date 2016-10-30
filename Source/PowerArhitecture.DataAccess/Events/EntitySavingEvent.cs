using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Event;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    /// <summary>
    /// Occurs before entity is saved without id
    /// </summary>
    public class EntitySavingEvent : IEvent
    {
        public EntitySavingEvent(SaveOrUpdateEvent @event)
        {
            Event = @event;
        }

        public SaveOrUpdateEvent Event { get; }
    }
}
