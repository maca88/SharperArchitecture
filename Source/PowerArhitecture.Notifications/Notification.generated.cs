using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Linq.Expressions;
using System.Reflection;
using FluentNHibernate.Automapping;

namespace PowerArhitecture.Notifications.Entities
{
	[GeneratedCode("T4Template", "1.0")]
	public partial class Notification
	{

		#region RecipientSearchPatterns

		private ISet<NotificationSearchPattern> _recipientSearchPatterns;

        public virtual void AddRecipientSearchPattern(NotificationSearchPattern recipientSearchPattern)
        {
            this.AddOneToMany(o => o.RecipientSearchPatterns, recipientSearchPattern, o => o.Notification, o=> o.RemoveRecipientSearchPattern);
        }

        public virtual void RemoveRecipientSearchPattern(NotificationSearchPattern recipientSearchPattern)
        {
            this.RemoveOneToMany(o => o.RecipientSearchPatterns, recipientSearchPattern, o => o.Notification);
        }

		#endregion

		#region Recipients

		private ISet<NotificationRecipient> _recipients;

        public virtual void AddRecipient(NotificationRecipient recipient)
        {
            this.AddOneToMany(o => o.Recipients, recipient, o => o.Notification, o=> o.RemoveRecipient);
        }

        public virtual void RemoveRecipient(NotificationRecipient recipient)
        {
            this.RemoveOneToMany(o => o.Recipients, recipient, o => o.Notification);
        }

		#endregion

	}
}
