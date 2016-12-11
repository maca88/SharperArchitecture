using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Notifications.Entities;
using PowerArhitecture.Notifications.Specifications;

namespace PowerArhitecture.Notifications.Events
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
