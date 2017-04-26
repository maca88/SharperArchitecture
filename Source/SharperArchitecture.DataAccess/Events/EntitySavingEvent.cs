using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Event;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
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
