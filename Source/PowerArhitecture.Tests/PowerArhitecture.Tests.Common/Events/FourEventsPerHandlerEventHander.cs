using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.Tests.Common.Events
{
    public class FourEventsPerHandlerEvent : BaseEvent
    {
        public bool Success { get; set; }
    }

    public class FourEventsPerHandler2Event : BaseEvent
    {
        public bool Success { get; set; }
    }

    public class FourEventsPerHandler3Event : BaseEvent
    {
        public bool Success { get; set; }
    }

    public class FourEventsPerHandler4Event : BaseEvent
    {
        public bool Success { get; set; }
    }

    public class FourEventsPerHandlerEventHander : BaseEventsHandler<FourEventsPerHandlerEvent, FourEventsPerHandler2Event,
        FourEventsPerHandler3Event, FourEventsPerHandler4Event>
    {
        public override void Handle(FourEventsPerHandlerEvent @event)
        {
            @event.Success = true;
        }

        public override void Handle(FourEventsPerHandler2Event @event)
        {
            @event.Success = true;
        }

        public override void Handle(FourEventsPerHandler3Event @event)
        {
            @event.Success = true;
        }

        public override void Handle(FourEventsPerHandler4Event @event)
        {
            @event.Success = true;
        }
    }
}
