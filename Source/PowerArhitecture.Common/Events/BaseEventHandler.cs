using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Events
{
    public abstract class BaseEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent
    {
        public abstract void Handle(TEvent notification);

        public virtual Task HandleAsync(TEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                Handle(notification);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        Task ICancellableAsyncNotificationHandler<TEvent>.Handle(TEvent notification, CancellationToken cancellationToken)
        {
            return HandleAsync(notification, cancellationToken);
        }
    }
}
