﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Events
{
    public class TestAsyncEvent : IAsyncEvent
    {
        public TestAsyncEvent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class TestAsyncEventHandler : IAsyncEventHandler<TestAsyncEvent>
    {
        public async Task HandleAsync(TestAsyncEvent notification, CancellationToken cancellationToken)
        {
            await Task.Yield();
            ReceivedMesssage = notification.Message;
            CallCounter++;
        }

        public string ReceivedMesssage { get; private set; }

        public int CallCounter { get; set; }
    }
}
