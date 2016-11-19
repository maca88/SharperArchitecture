﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Notifications;
using PowerArhitecture.Tests.Notifications.Entities;

namespace PowerArhitecture.Tests.Notifications.Repositories
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
