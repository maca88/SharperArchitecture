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
    public class NotificationInsertingEventListener : BaseListener<EntitySavingEvent>
    {
        private readonly ISessionEventProvider _sessionEventProvider;
        private readonly IEventAggregator _eventAggregator;

        public NotificationInsertingEventListener(ISessionEventProvider sessionEventProvider, IEventAggregator eventAggregator)
        {
            _sessionEventProvider = sessionEventProvider;
            _eventAggregator = eventAggregator;
        }

        public override void Handle(EntitySavingEvent e)
        {
            var @event = e.Message;
            var notification = @event.Entity as INotificationInternal;
            if (notification == null) return;

            _sessionEventProvider.AddListener(SessionListenerType.AfterCommit, @event.Session,
                () => _eventAggregator.SendMessage(new NewNotificationEvent(notification)));
        }


        public override Task HandleAsync(EntitySavingEvent e)
        {
            var @event = e.Message;
            var notification = @event.Entity as INotificationInternal;
            if (notification == null)
            {
                return Task.CompletedTask;
            }
            _sessionEventProvider.AddListener(SessionListenerType.AfterCommit, @event.Session,
                () => _eventAggregator.SendMessageAsync(new NewNotificationEvent(notification)));
            return Task.CompletedTask;
        }
    }
}
