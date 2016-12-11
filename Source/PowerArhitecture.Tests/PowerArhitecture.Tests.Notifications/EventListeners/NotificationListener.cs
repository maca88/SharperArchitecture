using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Notifications.Events;

namespace PowerArhitecture.Tests.Notifications.EventListeners
{
    public class NotificationHandler : IEventHandler<NewNotificationEvent>
    {
        public void Handle(NewNotificationEvent message)
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
