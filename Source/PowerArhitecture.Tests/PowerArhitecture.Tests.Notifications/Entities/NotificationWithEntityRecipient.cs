using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Notifications.Entities;

namespace PowerArhitecture.Tests.Notifications.Entities
{
    public class NotificationWithEntityRecipient 
        : Notification<User, NotificationWithEntityRecipient, NotificationSearchPatternWithEntityRecipient, NotificationRecipientWithEntityRecipient>
    {
        public override Expression<Func<NotificationRecipientWithEntityRecipient, bool>> GetCompareRecipientExpression(User recipient)
        {
            return stringRecipient => stringRecipient.Recipient.Id == recipient.Id;
        }
    }
}
