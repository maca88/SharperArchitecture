using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.Notifications.Events;
using PowerArhitecture.Notifications.Specifications;
using NHibernate.Extensions;

namespace PowerArhitecture.Notifications.EventListeners
{
    public class NotificationInsertingEventHandler : BaseEventHandler<EntitySavingEvent>
    {
        private readonly IEventPublisher _eventPublisher;

        public NotificationInsertingEventHandler(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public override void Handle(EntitySavingEvent e)
        {
            var notification = e.Event.Entity as INotificationInternal;
            if (notification == null)
            {
                return;
            }
            e.Event.Session.Subscribe(o => o.Transaction.AfterCommit(success =>
            {
                if (success)
                {
                    _eventPublisher.Publish(new NewNotificationEvent(notification));
                }
            }));
        }
    }
}
