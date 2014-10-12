using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Notifications.Entities;

namespace PowerArhitecture.Notifications.Specifications
{
    public interface INotificationTask : ITask
    {
        void SendToClients(Notification notification);
    }
}