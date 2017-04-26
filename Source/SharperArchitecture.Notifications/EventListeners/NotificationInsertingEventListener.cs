using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Event;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Events;
using SharperArchitecture.Notifications.Events;
using SharperArchitecture.Notifications.Specifications;
using NHibernate.Extensions;

namespace SharperArchitecture.Notifications.EventListeners
{
    public class NotificationInsertingEventHandler : IEventHandler<EntitySavingEvent>, IAsyncEventHandler<EntitySavingAsyncEvent>
    {
        private readonly IEventPublisher _eventPublisher;

        public NotificationInsertingEventHandler(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public void Handle(EntitySavingEvent e)
        {
            Handle(e.Event);
        }

        public Task HandleAsync(EntitySavingAsyncEvent e, CancellationToken cancellationToken)
        {
            Handle(e.Event);
            return Task.CompletedTask;
        }

        private void Handle(SaveOrUpdateEvent @event)
        {
            var notification = @event.Entity as INotificationInternal;
            if (notification == null)
            {
                return;
            }
            @event.Session.Subscribe(o => o.Transaction.AfterCommit(success =>
            {
                if (success)
                {
                    _eventPublisher.Publish(new NewNotificationEvent(notification));
                }
            }));
        }
    }
}
