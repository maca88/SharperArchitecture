using System;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Events
{
    public class TestEvent : IEvent
    {
        public TestEvent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class TestEventHandler : IEventHandler<TestEvent>
    {
        public void Handle(TestEvent notification)
        {
            ReceivedMesssage = notification.Message;
            CallCounter++;
        }

        public string ReceivedMesssage { get; private set; }

        public static int CallCounter { get; set; }
    }
}
