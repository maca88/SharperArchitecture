using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;
using SharperArchitecture.Notifications.Specifications;

namespace SharperArchitecture.Notifications.Entities
{
    [Ignore]
    [Serializable]
    public abstract partial class NotificationRecipient<TRecipient, TNotification, TNotificationRecipient> : Entity, INotificationRecipient<TRecipient>
        where TNotification : Notification<TRecipient, TNotification, TNotificationRecipient>, new()
        where TNotificationRecipient : NotificationRecipient<TRecipient, TNotification, TNotificationRecipient>, new()
    {
        [Unique("Recipient")]
        public virtual TNotification Notification { get; set; }

        [Unique("Recipient")]
        public virtual TRecipient Recipient { get; set; }

        public virtual DateTime? ReadDate { get; set; }
    }
}
