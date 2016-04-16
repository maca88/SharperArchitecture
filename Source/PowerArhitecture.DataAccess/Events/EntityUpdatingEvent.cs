using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Event;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.DataAccess.Events
{
    public class EntityUpdatingEvent: BaseEvent<SaveOrUpdateEvent>
    {
        public EntityUpdatingEvent(SaveOrUpdateEvent message)
            : base(message)
        {
        }
    }
}
