using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Specifications
{
    public interface IAsyncEventHandler<in TEvent> where TEvent : IAsyncEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
