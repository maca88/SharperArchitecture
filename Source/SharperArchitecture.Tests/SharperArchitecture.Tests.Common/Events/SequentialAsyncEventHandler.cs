using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Tests.Common.Events
{
    public class SequentialAsyncEvent : IAsyncEvent
    {
        public Action<SequentialAsyncEventHandler> OnStarted { get; set; }

        public Action<SequentialAsyncEventHandler> OnCompleted { get; set; }
    }


    public class SequentialAsyncEventHandler : IAsyncEventHandler<SequentialAsyncEvent>
    {
        public async Task HandleAsync(SequentialAsyncEvent notification, CancellationToken cancellationToken)
        {
            notification.OnStarted(this);
            await Task.Delay(100, cancellationToken);
            notification.OnCompleted(this);
        }
    }

    public class SequentialAsync2EventHandler : SequentialAsyncEventHandler
    {
    }
}
