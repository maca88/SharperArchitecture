using PowerArhitecture.Common.Events;
using PowerArhitecture.Notifications.Entities;
using PowerArhitecture.Notifications.Specifications;

namespace PowerArhitecture.Notifications.Events
{
    public class NewNotificationEvent : MessageEvent<INotification>
    {
        public NewNotificationEvent(INotification message)
            : base(message)
        {
        }
    }
}
