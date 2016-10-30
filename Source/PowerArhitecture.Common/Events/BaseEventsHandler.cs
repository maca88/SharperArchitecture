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
    public abstract class BaseEventsHandler<TEvent, TEvent2, TEvent3, TEvent4> : BaseEventsHandler<TEvent, TEvent2, TEvent3>, IEventHandler<TEvent4>
        where TEvent : IEvent
        where TEvent2 : IEvent
        where TEvent3 : IEvent
        where TEvent4 : IEvent
    {
        public abstract void Handle(TEvent4 @event);

        public virtual Task HandleAsync(TEvent4 @event, CancellationToken cancellationToken)
        {
            Handle(@event);
            return Task.CompletedTask;
        }

        Task ICancellableAsyncNotificationHandler<TEvent4>.Handle(TEvent4 @event, CancellationToken cancellationToken)
        {
            return HandleAsync(@event, cancellationToken);
        }
    }

    public abstract class BaseEventsHandler<TEvent, TEvent2, TEvent3> : BaseEventsHandler<TEvent, TEvent2>, IEventHandler<TEvent3>
        where TEvent : IEvent
        where TEvent2 : IEvent
        where TEvent3 : IEvent
    {
        public abstract void Handle(TEvent3 @event);

        public virtual Task HandleAsync(TEvent3 @event, CancellationToken cancellationToken)
        {
            Handle(@event);
            return Task.CompletedTask;
        }

        Task ICancellableAsyncNotificationHandler<TEvent3>.Handle(TEvent3 @event, CancellationToken cancellationToken)
        {
            return HandleAsync(@event, cancellationToken);
        }
    }

    public abstract class BaseEventsHandler<TEvent, TEvent2> : BaseEventHandler<TEvent>, IEventHandler<TEvent2>
        where TEvent : IEvent
        where TEvent2 : IEvent
    {
        public abstract void Handle(TEvent2 @event);

        public virtual Task HandleAsync(TEvent2 @event, CancellationToken cancellationToken)
        {
            Handle(@event);
            return Task.CompletedTask;
        }

        Task ICancellableAsyncNotificationHandler<TEvent2>.Handle(TEvent2 @event, CancellationToken cancellationToken)
        {
            return HandleAsync(@event, cancellationToken);
        }
    }
}
