using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Notifications.Enums;

namespace PowerArhitecture.Notifications.Entities
{
    public partial class Notification : VersionedEntity
    {
        public virtual string Message { get; set; }

        public virtual string Url { get; set; }

        public virtual bool OpenInNewWindow { get; set; }

        public virtual NotificationType Type { get; set; }

        public virtual ISet<NotificationSearchPattern> RecipientSearchPatterns
        {
            get { return _recipientSearchPatterns ?? (_recipientSearchPatterns = new HashSet<NotificationSearchPattern>()); }
            set { _recipientSearchPatterns = value; }
        }

        public virtual NotificationRecipient GetNotificationRecipient(IUser recipient)
        {
            return Recipients.FirstOrDefault(o => o.Recipient.Id == recipient.Id);
        }

        /// <summary>
        /// This set will be filled automatically by RecipientSearchPatterns
        /// </summary>
        public virtual ISet<NotificationRecipient> Recipients
        {
            get { return _recipients ?? (_recipients = new HashSet<NotificationRecipient>()); }
            set { _recipients = value; }
        } 
    }
}
