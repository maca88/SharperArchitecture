using PowerArhitecture.Common.Events;
using PowerArhitecture.Notifications.Entities;

namespace PowerArhitecture.Notifications.Events
{
    public class NewNotificationEvent : BaseEvent<Notification>
    {
        public NewNotificationEvent(Notification message)
            : base(message)
        {
        }
    }
}
