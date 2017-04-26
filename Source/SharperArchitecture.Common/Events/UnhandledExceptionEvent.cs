using System;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Common.Events
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
