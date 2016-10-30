using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.Tests.Common.Events
{
    public class FaultyEvent : MessageEvent<string>
    {
        public FaultyEvent(string message) : base(message)
        {
        }
    }

    public class FaultyEventHandler : BaseEventHandler<FaultyEvent>
    {
        public override void Handle(FaultyEvent notification)
        {
            throw new InvalidOperationException("error");
        }
    }
}
