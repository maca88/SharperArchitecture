using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Notifications.Entities
{
    public partial class NotificationSearchPattern : Entity
    {
        public virtual Notification Notification { get; set; }

        public virtual string Pattern { get; set; }

        public virtual string RecipientSearchType { get; set; }
    }
}
