using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using NHibernate.Event;

namespace PowerArhitecture.DataAccess.Events
{
    public class EntityPreCollectionUpdatingEvent : BaseEvent<PreCollectionUpdateEvent>
    {
        public EntityPreCollectionUpdatingEvent(PreCollectionUpdateEvent message) : base(message)
        {
        }
    }
}
