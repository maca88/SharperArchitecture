using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Notifications.Enums;
using PowerArhitecture.Notifications.Specifications;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Notifications.Entities
{
    [Ignore]
    [Serializable]
    public abstract partial class Notification<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient> : VersionedEntity, INotificationInternal, INotification<TRecipient> 
        where TNotification : Notification<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>, new()
        where TNotificationSearchPattern : NotificationSearchPattern<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>, new()
        where TNotificationRecipient : NotificationRecipient<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>, new()
    {
        
        [Length(int.MaxValue)]
        public virtual string Message { get; set; }

        public virtual NotificationType Type { get; set; }
        
        public virtual ISet<TNotificationSearchPattern> RecipientSearchPatterns
        {
            get { return _recipientSearchPatterns ?? (_recipientSearchPatterns = new HashSet<TNotificationSearchPattern>()); }
            set { _recipientSearchPatterns = value; }
        }

        public virtual TNotificationRecipient GetNotificationRecipient(TRecipient recipient)
        {
            return Recipients.FirstOrDefault(GetCompareRecipientExpression(recipient).Compile());
        }

        public abstract Expression<Func<TNotificationRecipient, bool>> GetCompareRecipientExpression(TRecipient recipient);

        /// <summary>
        /// This set will be filled automatically by RecipientSearchPatterns
        /// </summary>
        public virtual ISet<TNotificationRecipient> Recipients
        {
            get { return _recipients ?? (_recipients = new HashSet<TNotificationRecipient>()); }
            set { _recipients = value; }
        }

        public virtual IEnumerable<INotificationSearchPattern> GetSearchPatterns()
        {
            return RecipientSearchPatterns;
        }

        public virtual void AddRecipient(object recipient)
        {
            AddRecipient(new TNotificationRecipient
            {
                Recipient = (TRecipient)recipient
            });
        }

        public virtual IEnumerable<TRecipient> GetRecipients()
        {
            return Recipients.Select(o => o.Recipient);
        }
    }
}
