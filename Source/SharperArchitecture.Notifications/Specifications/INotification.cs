using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Notifications.Enums;

namespace SharperArchitecture.Notifications.Specifications
{
    public interface INotification
    {
        string Message { get; set; }

        NotificationType Type { get; set; }

    }

    public interface INotificationInternal : INotification
    {
        void AddRecipient(object recipient);
    }

    public interface INotification<out TRecipient> : INotification
    {
        IEnumerable<TRecipient> GetRecipients();
    }
}
