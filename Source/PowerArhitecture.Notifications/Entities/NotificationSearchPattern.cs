using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Notifications.Specifications;

namespace PowerArhitecture.Notifications.Entities
{
    [Ignore]
    [Serializable]
    public abstract partial class NotificationSearchPattern<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient> : Entity, INotificationSearchPattern
        where TNotification : Notification<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>, new()
        where TNotificationSearchPattern : NotificationSearchPattern<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>, new()
        where TNotificationRecipient : NotificationRecipient<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>, new()
    {
        public virtual TNotification Notification { get; set; }

        public virtual string Pattern { get; set; }

        public virtual string RecipientSearchType { get; set; }
    }
}
