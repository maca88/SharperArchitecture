using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Tests.Common.Events
{
    public class MultipleEventsPerHandlerEvent : IEvent
    {
        public bool Success { get; set; }
    }

    public class MultipleEventsPerHandler2Event : IEvent
    {
        public bool Success { get; set; }
    }

    public class MultipleEventsPerHandlerEventHander : 
        IEventHandler<MultipleEventsPerHandlerEvent>,
        IEventHandler<MultipleEventsPerHandler2Event>
    {
        public void Handle(MultipleEventsPerHandlerEvent @event)
        {
            @event.Success = true;
        }

        public void Handle(MultipleEventsPerHandler2Event @event)
        {
            @event.Success = true;
        }
    }
}
