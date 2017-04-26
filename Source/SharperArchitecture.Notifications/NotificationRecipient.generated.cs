using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Linq.Expressions;
using System.Reflection;
using FluentNHibernate.Automapping;
namespace SharperArchitecture.Notifications.Entities
{
	[GeneratedCode("T4Template", "1.0")]
    public partial class NotificationRecipient<TRecipient, TNotification, TNotificationRecipient>
	{

		#region Notification

        [ReadOnly(true)]
        public virtual long? NotificationId { get; set; }

        public virtual void SetNotification(TNotification notification)
        {
            ((TNotificationRecipient)this).SetManyToOne<TNotificationRecipient, TNotification>(o => o.Notification, notification, o => o.RemoveRecipient, o => o.Recipients);
        }

        public virtual void UnsetNotification()
        {
            this.UnsetManyToOne(o => o.Notification, o => o.Recipients);
        }

		#endregion

	}
}
