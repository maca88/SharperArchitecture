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
	public partial class Permission
	{

		#region RolePermissions

		private ISet<RolePermission> _rolePermissions;

        public virtual void AddRolePermission(RolePermission rolePermission)
        {
            this.AddOneToMany(o => o.RolePermissions, rolePermission, o => o.Permission, o=> o.RemoveRolePermission);
        }

        public virtual void RemoveRolePermission(RolePermission rolePermission)
        {
            this.RemoveOneToMany(o => o.RolePermissions, rolePermission, o => o.Permission);
        }

		#endregion

	}
}
