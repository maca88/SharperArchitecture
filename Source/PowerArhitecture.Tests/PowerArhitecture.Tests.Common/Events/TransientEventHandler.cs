using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Enums;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using SimpleInjector;

namespace PowerArhitecture.Tests.Common.Events
{
    public class TransientEvent : IEvent
    {
        public TransientEvent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    [Lifetime(Lifetime.Transient)]
    public class TransientEventHandler : IEventHandler<TransientEvent>
    {
        public TransientEventHandler()
        {
            CreatedTimes++;
        }

        public static int CreatedTimes;
        public static int CallCounter;

        public void Handle(TransientEvent notification)
        {
            CallCounter++;
        }
    }
}
