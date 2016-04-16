using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Notifications.Specifications
{
    interface INotificationRecipient<TRecipient>
    {
        TRecipient Recipient { get; set; }

        DateTime? ReadDate { get; set; }
    }
}
