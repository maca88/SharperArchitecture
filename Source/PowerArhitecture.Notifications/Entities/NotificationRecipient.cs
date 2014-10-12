using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.Notifications.Entities
{
    public partial class NotificationRecipient : Entity
    {
        [Unique("Recipient")]
        public virtual Notification Notification { get; set; }

        [Unique("Recipient")]
        public virtual IUser Recipient { get; set; }

        public virtual DateTime? ReadDate { get; set; }
    }
}
