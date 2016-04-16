using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Notifications.Specifications;

namespace PowerArhitecture.Notifications.Entities
{
    [Ignore]
    [Serializable]
    public abstract partial class NotificationRecipient<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient> : VersionedEntity, INotificationRecipient<TRecipient>
        where TNotification : Notification<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>, new()
        where TNotificationSearchPattern : NotificationSearchPattern<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>, new()
        where TNotificationRecipient : NotificationRecipient<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>, new()
    {
        [Unique("Recipient")]
        public virtual TNotification Notification { get; set; }

        [Unique("Recipient")]
        public virtual TRecipient Recipient { get; set; }

        public virtual DateTime? ReadDate { get; set; }
    }
}
