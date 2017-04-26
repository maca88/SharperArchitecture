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
    public partial class Notification<TRecipient, TNotification, TNotificationRecipient>
	{

		#region Recipients

        private ISet<TNotificationRecipient> _recipients;

        public virtual void AddRecipient(TNotificationRecipient recipient)
        {
            this.AddOneToMany(o => o.Recipients, recipient, o => o.Notification, o=> o.RemoveRecipient);
        }

        public virtual void RemoveRecipient(TNotificationRecipient recipient)
        {
            this.RemoveOneToMany(o => o.Recipients, recipient, o => o.Notification);
        }

		#endregion

	}
}
