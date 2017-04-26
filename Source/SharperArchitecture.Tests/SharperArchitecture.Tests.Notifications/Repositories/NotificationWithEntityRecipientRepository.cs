using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Notifications;
using SharperArchitecture.Tests.Notifications.Entities;

namespace SharperArchitecture.Tests.Notifications.Repositories
{
    public class NotificationWithEntityRecipientRepository
        : NotificationRepository<User, NotificationWithEntityRecipient, NotificationRecipientWithEntityRecipient>
    {
        public NotificationWithEntityRecipientRepository(ISession session, ILogger logger) 
            : base(session, logger)
        {
        }
    }
}
