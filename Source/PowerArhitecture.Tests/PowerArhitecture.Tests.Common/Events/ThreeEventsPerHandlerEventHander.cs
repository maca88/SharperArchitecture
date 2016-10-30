using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Events
{
    public class ThreeEventsPerHandlerEvent : IEvent
    {
        public bool Success { get; set; }
    }

    public class ThreeEventsPerHandler2Event : IEvent
    {
        public bool Success { get; set; }
    }

    public class ThreeEventsPerHandler3Event : IEvent
    {
        public bool Success { get; set; }
    }

    public class ThreeEventsPerHandlerEventHander : BaseEventsHandler<ThreeEventsPerHandlerEvent, ThreeEventsPerHandler2Event, ThreeEventsPerHandler3Event>
    {
        public override void Handle(ThreeEventsPerHandlerEvent @event)
        {
            @event.Success = true;
        }

        public override void Handle(ThreeEventsPerHandler2Event @event)
        {
            @event.Success = true;
        }

        public override void Handle(ThreeEventsPerHandler3Event @event)
        {
            @event.Success = true;
        }
    }
}
