using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Events
{
    public class SequentialAsyncEvent : IEvent
    {
        public Action<SequentialAsyncEventHandler> OnStarted { get; set; }

        public Action<SequentialAsyncEventHandler> OnCompleted { get; set; }
    }


    public class SequentialAsyncEventHandler : BaseEventHandler<SequentialAsyncEvent>
    {
        public override void Handle(SequentialAsyncEvent notification)
        {
            throw new NotImplementedException();
        }

        public override async Task HandleAsync(SequentialAsyncEvent notification, CancellationToken cancellationToken)
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
