using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Events
{
    public class SequentialAsyncPriorityEvent : IEvent
    {
        public Action<SequentialAsyncPriorityEventHandler> OnStarted { get; set; }

        public Action<SequentialAsyncPriorityEventHandler> OnCompleted { get; set; }
    }


    public class SequentialAsyncPriorityEventHandler : BaseEventHandler<SequentialAsyncPriorityEvent>
    {
        public override void Handle(SequentialAsyncPriorityEvent notification)
        {
            throw new NotImplementedException();
        }

        public override async Task HandleAsync(SequentialAsyncPriorityEvent notification, CancellationToken cancellationToken)
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
