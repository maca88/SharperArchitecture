using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Common.Enums;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SimpleInjector;

namespace SharperArchitecture.Tests.Common.Events
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
