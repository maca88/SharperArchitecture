using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Linq.Expressions;
using System.Reflection;
using FluentNHibernate.Automapping;

namespace PowerArhitecture.Authentication.Entities
{
	[GeneratedCode("T4Template", "1.0")]
	public partial class UserSetting
	{

		#region User

        [ReadOnly(true)]
        public virtual long? UserId { get; set; }

        public virtual void SetUser(User user)
        {
            this.SetManyToOne(o => o.User, user, o => o.RemoveSetting, o => o.Settings);
        }

        public virtual void UnsetUser()
        {
            this.UnsetManyToOne(o => o.User, o => o.Settings);
        }

		#endregion

	}
}
