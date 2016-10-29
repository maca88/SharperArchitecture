using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Notifications.Events;

namespace PowerArhitecture.Tests.Notifications.EventListeners
{
    public class NotificationListener : BaseListener<NewNotificationEvent>
    {
        public override void Handle(NewNotificationEvent message)
        {
            NotificationEvent = message;
        }

        public NewNotificationEvent NotificationEvent;

        public void Reset()
        {
            NotificationEvent = null;
        }
    }
}
