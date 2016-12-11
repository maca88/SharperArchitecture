﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Events
{
    public class FaultyAsyncEvent : IAsyncEvent
    {
        public FaultyAsyncEvent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class FaultyAsyncEventHandler : IAsyncEventHandler<FaultyAsyncEvent>
    {
        public Task HandleAsync(FaultyAsyncEvent notification, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("error");
        }
    }
}
