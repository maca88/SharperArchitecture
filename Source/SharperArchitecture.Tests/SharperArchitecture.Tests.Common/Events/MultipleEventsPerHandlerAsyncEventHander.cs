using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Tests.Common.Events
{
    public class MultipleEventsPerHandlerAsyncEvent : IAsyncEvent
    {
        public bool Success { get; set; }
    }

    public class MultipleEventsPerHandler2AsyncEvent : IAsyncEvent
    {
        public bool Success { get; set; }
    }

    public class MultipleEventsPerHandlerAsyncEventHander :
        IAsyncEventHandler<MultipleEventsPerHandlerAsyncEvent>,
        IAsyncEventHandler<MultipleEventsPerHandler2AsyncEvent>
    {
        public async Task HandleAsync(MultipleEventsPerHandlerAsyncEvent @event, CancellationToken cancellationToken)
        {
            await Task.Yield();
            @event.Success = true;
        }

        public async Task HandleAsync(MultipleEventsPerHandler2AsyncEvent @event, CancellationToken cancellationToken)
        {
            await Task.Yield();
            @event.Success = true;
        }
    }
}
