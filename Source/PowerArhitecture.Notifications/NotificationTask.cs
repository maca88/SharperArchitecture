using PowerArhitecture.Common.Events;
using PowerArhitecture.Notifications.Entities;
using PowerArhitecture.Notifications.Events;
using PowerArhitecture.Notifications.Specifications;

namespace PowerArhitecture.Notifications
{
    public class NotificationTask : INotificationTask
    {
        private readonly IEventAggregator _eventAggregator;

        public NotificationTask(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void SendToClients(Notification notification)
        {
            _eventAggregator.SendMessage(new NewNotificationEvent(notification));
        }
    }
}
