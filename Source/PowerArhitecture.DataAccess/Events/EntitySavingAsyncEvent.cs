using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Event;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    /// <summary>
    /// Occurs before entity is saved without id
    /// </summary>
    public class EntitySavingAsyncEvent : IAsyncEvent
    {
        public EntitySavingAsyncEvent(SaveOrUpdateEvent @event)
        {
            Event = @event;
        }

        public SaveOrUpdateEvent Event { get; }
    }
}
