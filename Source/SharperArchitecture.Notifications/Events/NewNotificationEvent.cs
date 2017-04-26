using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Notifications.Entities;
using SharperArchitecture.Notifications.Specifications;

namespace SharperArchitecture.Notifications.Events
{
    public class NewNotificationEvent : IEvent
    {
        public NewNotificationEvent(INotification notification)
        {
            Notification = notification;
        }

        public INotification Notification { get; }
    }
}
