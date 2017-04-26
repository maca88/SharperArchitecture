using System;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Tests.Common.Events
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
