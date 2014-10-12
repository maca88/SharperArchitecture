using System;
using System.Collections.Generic;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Notifications.Entities;
using PowerArhitecture.Notifications.Managers;
using PowerArhitecture.Notifications.Specifications;
using Ninject;
using Ninject.Syntax;

namespace PowerArhitecture.Notifications.EventListeners
{
    public class NotificationInsertingEventListener : IListener<EntityPreInsertingEvent>
    {
        private readonly ISessionEventListener _sessionEventListener;
        private readonly INotificationTask _notificationTask;
        private readonly IResolutionRoot _resolutionRoot;

        public NotificationInsertingEventListener(ISessionEventListener sessionEventListener, IResolutionRoot resolutionRoot,
            INotificationTask notificationTask)
        {
            _sessionEventListener = sessionEventListener;
            _notificationTask = notificationTask;
            _resolutionRoot = resolutionRoot;
        }

        public void Handle(EntityPreInsertingEvent e)
        {
            var @event = e.Message;
            var notification = @event.Entity as Notification;
            if (notification == null) return;

            var recipients = new HashSet<IUser>(); //this will take care of duplicates

            foreach (var searchPattern in notification.RecipientSearchPatterns)
            {
                if (!RecipientSearchQueryManager.ContainsQueryName(searchPattern.RecipientSearchType))
                    throw new NotSupportedException("SearchType: " + searchPattern.RecipientSearchType);

                foreach (var recipient in _resolutionRoot.Get<IRecipientSearchQuery>(searchPattern.RecipientSearchType).GetRecipients(searchPattern.Pattern))
                {
                    recipients.Add(recipient);
                }
            }

            foreach (var recipient in recipients)
            {
                notification.AddRecipient(new NotificationRecipient
                {
                    Recipient = recipient
                });
            }
            _sessionEventListener.AddAListener(SessionListenerType.AfterCommit, @event.Session, 
                () => _notificationTask.SendToClients(notification));

        }
    }
}
