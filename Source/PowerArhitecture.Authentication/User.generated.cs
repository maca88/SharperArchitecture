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
	public partial class User
	{

		#region UserRoles

		private ISet<UserRole> _userRoles;

        public virtual void AddUserRole(UserRole userRole)
        {
            this.AddOneToMany(o => o.UserRoles, userRole, o => o.User, o=> o.RemoveUserRole);
        }

        public virtual void RemoveUserRole(UserRole userRole)
        {
            this.RemoveOneToMany(o => o.UserRoles, userRole, o => o.User);
        }

		#endregion

		#region Organization

        [ReadOnly(true)]
        public virtual long? OrganizationId { get; set; }

		#endregion

		#region Claims

		private ISet<UserClaim> _claims;

        public virtual void AddClaim(UserClaim claim)
        {
            this.AddOneToMany(o => o.Claims, claim, o => o.User, o=> o.RemoveClaim);
        }

        public virtual void RemoveClaim(UserClaim claim)
        {
            this.RemoveOneToMany(o => o.Claims, claim, o => o.User);
        }

		#endregion

		#region Logins

		private ISet<UserLogin> _logins;

        public virtual void AddLogin(UserLogin login)
        {
            this.AddOneToMany(o => o.Logins, login, o => o.User, o=> o.RemoveLogin);
        }

        public virtual void RemoveLogin(UserLogin login)
        {
            this.RemoveOneToMany(o => o.Logins, login, o => o.User);
        }

		#endregion

		#region Settings

		private ISet<UserSetting> _settings;

        public virtual void AddSetting(UserSetting setting)
        {
            this.AddOneToMany(o => o.Settings, setting, o => o.User, o=> o.RemoveSetting);
        }

        public virtual void RemoveSetting(UserSetting setting)
        {
            this.RemoveOneToMany(o => o.Settings, setting, o => o.User);
        }

		#endregion

	}
}
