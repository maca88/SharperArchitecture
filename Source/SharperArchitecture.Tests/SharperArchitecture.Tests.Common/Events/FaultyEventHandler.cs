using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Tests.Common.Events
{
    public class FaultyEvent : IEvent
    {
        public FaultyEvent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class FaultyEventHandler : IEventHandler<FaultyEvent>
    {
        public void Handle(FaultyEvent notification)
        {
            throw new InvalidOperationException("error");
        }
    }
}
