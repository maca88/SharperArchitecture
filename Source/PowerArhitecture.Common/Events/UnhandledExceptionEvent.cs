using System;

namespace PowerArhitecture.Common.Events
{
    public class UnhandledExceptionEvent : BaseEvent<Exception>
    {
        public UnhandledExceptionEvent(Exception message) : base(message)
        {
        }
    }
}
