using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Notifications.Events;

namespace SharperArchitecture.Tests.Notifications.EventListeners
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
