using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Notifications.Entities;

namespace SharperArchitecture.Tests.Notifications.Entities
{
    public class NotificationRecipientWithEntityRecipient
        : NotificationRecipient<User, NotificationWithEntityRecipient, NotificationRecipientWithEntityRecipient>
    {
    }
}
