using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Events
{
    public class FourEventsPerHandlerEvent : IEvent
    {
        public bool Success { get; set; }
    }

    public class FourEventsPerHandler2Event : IEvent
    {
        public bool Success { get; set; }
    }

    public class FourEventsPerHandler3Event : IEvent
    {
        public bool Success { get; set; }
    }

    public class FourEventsPerHandler4Event : IEvent
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

        public override void Handle(FourEventsPerHandler4Event e)
        {
            e.Success = true;
        }
    }
}
