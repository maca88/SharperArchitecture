using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Notifications.Entities;

namespace SharperArchitecture.Tests.Notifications.Entities
{
    public class NotificationWithEntityRecipient 
        : Notification<User, NotificationWithEntityRecipient, NotificationRecipientWithEntityRecipient>
    {
        public override Expression<Func<NotificationRecipientWithEntityRecipient, bool>> GetCompareRecipientExpression(User recipient)
        {
            return stringRecipient => stringRecipient.Recipient.Id == recipient.Id;
        }
    }
}
