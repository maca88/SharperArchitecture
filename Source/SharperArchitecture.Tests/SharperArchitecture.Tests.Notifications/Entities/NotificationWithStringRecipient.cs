using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Notifications.Entities;

namespace SharperArchitecture.Tests.Notifications.Entities
{
    public class NotificationWithStringRecipient 
        : Notification<string, NotificationWithStringRecipient, NotificationRecipientWithStringRecipient>
    {
        public override Expression<Func<NotificationRecipientWithStringRecipient, bool>> GetCompareRecipientExpression(string recipient)
        {
            return stringRecipient => stringRecipient.Recipient == recipient;
        }
    }
}
