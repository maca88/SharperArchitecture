using System;

namespace PowerArhitecture.Common.Events
{
    public class UnhandledExceptionEvent : MessageEvent<Exception>
    {
        public UnhandledExceptionEvent(Exception message) : base(message)
        {
        }
    }
}
