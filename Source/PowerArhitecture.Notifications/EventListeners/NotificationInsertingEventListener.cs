using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Wrappers;
using PowerArhitecture.Notifications.Entities;
using PowerArhitecture.Notifications.Events;
using PowerArhitecture.Notifications.Specifications;
using Ninject;
using Ninject.Syntax;

namespace PowerArhitecture.Notifications.EventListeners
{
    public class NotificationInsertingEventHandler : BaseEventHandler<EntitySavingEvent>
    {
        private readonly ISessionSubscriptionManager _sessionSubscriptionManager;
        private readonly IEventPublisher _eventPublisher;

        public NotificationInsertingEventHandler(ISessionSubscriptionManager sessionSubscriptionManager, IEventPublisher eventPublisher)
        {
            _sessionSubscriptionManager = sessionSubscriptionManager;
            _eventPublisher = eventPublisher;
        }

        public override void Handle(EntitySavingEvent e)
        {
            var @event = e.Message;
            var notification = @event.Entity as INotificationInternal;
            if (notification == null)
            {
                return;
            }

            _sessionSubscriptionManager.Subscribe(SessionSubscription.AfterCommit, @event.Session,
                () => _eventPublisher.Publish(new NewNotificationEvent(notification)));
        }
    }
}
