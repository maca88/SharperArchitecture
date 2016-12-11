using System;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Events
{
    public class UnhandledExceptionEvent : IEvent
    {
        public UnhandledExceptionEvent(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
