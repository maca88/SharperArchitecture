using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.Common.Specifications
{
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>, ICancellableAsyncNotificationHandler<TEvent>
        where TEvent : IEvent
    { }
}
