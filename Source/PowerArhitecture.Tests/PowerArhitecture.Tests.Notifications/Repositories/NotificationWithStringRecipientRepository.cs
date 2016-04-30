using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Ninject.Extensions.Logging;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Notifications;
using PowerArhitecture.Tests.Notifications.Entities;

namespace PowerArhitecture.Tests.Notifications.Repositories
{
    public class NotificationWithStringRecipientRepository
        : NotificationRepository<string, NotificationWithStringRecipient, NotificationRecipientWithStringRecipient>
    {
        public NotificationWithStringRecipientRepository(ISession session, ILogger logger, ISessionEventProvider sessionEventProvider) 
            : base(session, logger, sessionEventProvider)
        {
        }
    }
}
