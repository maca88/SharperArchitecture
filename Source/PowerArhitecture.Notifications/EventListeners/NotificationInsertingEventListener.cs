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
    public class NotificationInsertingEventListener : IListener<EntitySavingEvent>
    {
        private readonly ISessionEventProvider _sessionEventProvider;
        private readonly IResolutionRoot _resolutionRoot;
        private readonly IEventAggregator _eventAggregator;

        public NotificationInsertingEventListener(ISessionEventProvider sessionEventProvider, IResolutionRoot resolutionRoot, 
            IEventAggregator eventAggregator)
        {
            _sessionEventProvider = sessionEventProvider;
            _resolutionRoot = resolutionRoot;
            _eventAggregator = eventAggregator;
        }

        public void Handle(EntitySavingEvent e)
        {
            var @event = e.Message;
            var notification = @event.Entity as INotificationInternal;
            if (notification == null) return;

            _sessionEventProvider.AddAListener(SessionListenerType.AfterCommit, @event.Session,
                () => _eventAggregator.SendMessageAsync(new NewNotificationEvent(notification)));

        }
    }
}
