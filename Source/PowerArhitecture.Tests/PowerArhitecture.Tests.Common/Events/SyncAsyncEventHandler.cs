using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Events
{
    public class SyncAsyncEvent : IEvent, IAsyncEvent
    {
        public int Counter { get; set; }
    }

    public class SyncAsyncEventHandler : IEventHandler<SyncAsyncEvent>, IAsyncEventHandler<SyncAsyncEvent>
    {
        public void Handle(SyncAsyncEvent @event)
        {
            @event.Counter++;
        }

        public async Task HandleAsync(SyncAsyncEvent @event, CancellationToken cancellationToken)
        {
            await Task.Yield();
            @event.Counter++;
        }
    }
}
