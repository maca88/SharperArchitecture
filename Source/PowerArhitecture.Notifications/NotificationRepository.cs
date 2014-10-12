using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Notifications.Entities;
using PowerArhitecture.Validation.Specifications;
using NHibernate;
using Ninject.Extensions.Logging;

namespace PowerArhitecture.Notifications
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly IUserCache _userCache;

        public NotificationRepository(ISession session, ILogger logger, ISessionEventListener sessionEventListener,IUserCache userCache) 
            : base(session, logger, sessionEventListener)
        {
            _userCache = userCache;
        }

        public ICollection<Notification> GetUnReadNotifications()
        {
            var currentUser = _userCache.GetCurrentUser();
            if(currentUser == null)
                return new List<Notification>();
            return Session.QueryOver<Notification>()
                          .Inner.JoinQueryOver<NotificationRecipient>(o => o.Recipients)
                          .Where(o => o.ReadDate == null)
                          .Where(o => o.Recipient.Id == currentUser.Id)
                          .List();
        }




    }

    public interface INotificationRepository : IRepository<Notification>
    {
        ICollection<Notification> GetUnReadNotifications();
    }
}
