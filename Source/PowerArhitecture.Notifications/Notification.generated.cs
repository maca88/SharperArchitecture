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
    public partial class Notification<TRecipient, TNotification, TNotificationSearchPattern, TNotificationRecipient>
	{

		#region RecipientSearchPatterns

        private ISet<TNotificationSearchPattern> _recipientSearchPatterns;

        public virtual void AddRecipientSearchPattern(TNotificationSearchPattern recipientSearchPattern)
        {
            this.AddOneToMany(o => o.RecipientSearchPatterns, recipientSearchPattern, o => o.Notification, o=> o.RemoveRecipientSearchPattern);
        }

        public virtual void RemoveRecipientSearchPattern(TNotificationSearchPattern recipientSearchPattern)
        {
            this.RemoveOneToMany(o => o.RecipientSearchPatterns, recipientSearchPattern, o => o.Notification);
        }

		#endregion

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
