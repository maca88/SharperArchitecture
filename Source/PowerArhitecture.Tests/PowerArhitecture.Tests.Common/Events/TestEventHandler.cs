using System;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.Tests.Common.Events
{
    public class TestEvent : BaseEvent<string>
    {
        public TestEvent(string message) : base(message)
        {
        }
    }

    public class TestEventHandler : BaseEventHandler<TestEvent>
    {
        public override void Handle(TestEvent notification)
        {
            ReceivedMesssage = notification.Message;
            CallCounter++;
        }

        public string ReceivedMesssage { get; private set; }

        public int CallCounter { get; set; }
    }
}
