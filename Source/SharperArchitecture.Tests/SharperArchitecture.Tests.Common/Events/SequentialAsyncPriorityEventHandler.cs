using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Tests.Common.Events
{
    public class SequentialAsyncPriorityEvent : IAsyncEvent
    {
        public Action<SequentialAsyncPriorityEventHandler> OnStarted { get; set; }

        public Action<SequentialAsyncPriorityEventHandler> OnCompleted { get; set; }
    }


    public class SequentialAsyncPriorityEventHandler : IAsyncEventHandler<SequentialAsyncPriorityEvent>
    {
        public async Task HandleAsync(SequentialAsyncPriorityEvent notification, CancellationToken cancellationToken)
        {
            notification.OnStarted(this);
            await Task.Delay(100, cancellationToken);
            notification.OnCompleted(this);
        }
    }

    [Priority(20)]
    public class SequentialAsyncPriority2EventHandler : SequentialAsyncPriorityEventHandler
    {
    }
}
