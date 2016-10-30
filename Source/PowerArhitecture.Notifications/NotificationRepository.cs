using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Notifications.Entities;
using PowerArhitecture.Validation.Specifications;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using PowerArhitecture.Domain;
using PowerArhitecture.Notifications.Specifications;

namespace PowerArhitecture.Notifications
{
    public abstract class NotificationRepository<TRecipient, TNotification, TNotificationRecipient> 
        : Repository<TNotification>, INotificationRepository<TRecipient, TNotification>
        where TNotification : Notification<TRecipient, TNotification, TNotificationRecipient>, new()
        where TNotificationRecipient : NotificationRecipient<TRecipient, TNotification, TNotificationRecipient>, new()
    {
        //Create a fake notification instance as the instance contains the recipient compare expression
        private readonly TNotification _notification = new TNotification();

        protected NotificationRepository(ISession session, ILogger logger) 
            : base(session, logger)
        {
        }

        /// <summary>
        /// Query unread notifications for the specified user
        /// </summary>
        /// <param name="recipient"></param>
        /// <returns></returns>
        public virtual IQueryable<TNotification> QueryUnReadNotifications(TRecipient recipient)
        {
            return Session.Query<TNotificationRecipient>()
                .Where(o => o.ReadDate == null)
                .Where(_notification.GetCompareRecipientExpression(recipient))
                .Select(o => o.Notification);
        }

        public virtual IEnumerable<TNotification> GetUnReadNotifications(TRecipient recipient)
        {
            return QueryUnReadNotifications(recipient).ToList();
        }

        public virtual IEnumerable<TNotification> MarkAllNotificationsAsReaded(TRecipient recipient)
        {
            var results = new List<TNotification>();
            var notificationRecipients = Session.Query<TNotificationRecipient>()
                .Fetch(o => o.Notification)
                .Where(_notification.GetCompareRecipientExpression(recipient))
                .ToList();
            if (!notificationRecipients.Any()) return results;
            foreach (var notificationRecipient in notificationRecipients)
            {
                notificationRecipient.ReadDate = DateTime.UtcNow;
                results.Add(notificationRecipient.Notification);
            }
            return results;
        }

        public virtual TNotification MarkNotificationAsReaded(long notificationId, TRecipient recipient)
        {
            var notification = Session.Query<TNotification>()
                .FetchMany(o => o.Recipients).ThenFetch(o => o.Recipient)
                .FirstOrDefault(o => o.Id == notificationId);
            if (notification == null) return null;
            var notifrecipient = notification.Recipients.FirstOrDefault(_notification.GetCompareRecipientExpression(recipient).Compile());
            if (notifrecipient == null) return null;
            notifrecipient.ReadDate = DateTime.UtcNow;
            return notification;
        }

    }

    public interface INotificationRepository<in TRecipient, TNotification> : IRepository<TNotification>
        where TNotification : class, INotification, IEntity<long>, new()
    {
        IEnumerable<TNotification> GetUnReadNotifications(TRecipient recipient);

        IQueryable<TNotification> QueryUnReadNotifications(TRecipient recipient);

        TNotification MarkNotificationAsReaded(long notificationId, TRecipient recipient);

        IEnumerable<TNotification> MarkAllNotificationsAsReaded(TRecipient recipient);
    }
}
