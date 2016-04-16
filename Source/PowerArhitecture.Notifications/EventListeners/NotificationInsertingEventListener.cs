using System;
using System.Collections.Generic;
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
        private readonly ISessionManager _sessionManager;
        private readonly IEventAggregator _eventAggregator;

        public NotificationInsertingEventListener(ISessionEventProvider sessionEventProvider, IResolutionRoot resolutionRoot, 
            IEventAggregator eventAggregator, ISessionManager sessionManager)
        {
            _sessionEventProvider = sessionEventProvider;
            _resolutionRoot = resolutionRoot;
            _eventAggregator = eventAggregator;
            _sessionManager = sessionManager;
        }

        public void Handle(EntitySavingEvent e)
        {
            var @event = e.Message;
            var notification = @event.Entity as INotificationInternal;
            if (notification == null) return;

            var resRoot = _resolutionRoot;

            //Try to get the context resolution root in order to prevent creating another session when getting the recipient search query instance
            var sessionInfo = _sessionManager.GetSessionInfo(@event.Session);
            if (sessionInfo != null && sessionInfo.SessionProperties.SessionResolutionRoot != null)
            {
                resRoot = sessionInfo.SessionProperties.SessionResolutionRoot;
            }

            var recipients = new HashSet<object>(); //this will take care of duplicates

            foreach (var searchPattern in notification.GetSearchPatterns())
            {
                var recSearchPattern = resRoot.TryGet<IRecipientSearchQuery>(searchPattern.RecipientSearchType);

                if (recSearchPattern == null)
                    throw new NotSupportedException("SearchType: " + searchPattern.RecipientSearchType);

                foreach (var recipient in recSearchPattern.GetRecipients(searchPattern.Pattern))
                {
                    recipients.Add(recipient);
                }
            }

            foreach (var recipient in recipients)
            {
                notification.AddRecipient(recipient);
            }

            _sessionEventProvider.AddAListener(SessionListenerType.AfterCommit, @event.Session,
                () => _eventAggregator.SendMessage(new NewNotificationEvent(notification)));

        }
    }
}
