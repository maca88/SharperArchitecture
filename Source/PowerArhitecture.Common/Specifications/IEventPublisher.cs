using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.Common.Specifications
{
    public interface IEventPublisher
    {
        void Publish(IEvent @event);

        Task PublishAsync(IAsyncEvent @event);

        Task PublishAsync(IAsyncEvent @event, CancellationToken cancellationToken);
    }
}
