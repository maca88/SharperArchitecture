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
	public partial class RolePermission
	{

		#region Role

        [ReadOnly(true)]
        public virtual long? RoleId { get; set; }

        public virtual void SetRole(Role role)
        {
            this.SetManyToOne(o => o.Role, role, o => o.RemoveRolePermission, o => o.RolePermissions);
        }

        public virtual void UnsetRole()
        {
            this.UnsetManyToOne(o => o.Role, o => o.RolePermissions);
        }

		#endregion

		#region Permission

        [ReadOnly(true)]
        public virtual long? PermissionId { get; set; }

        public virtual void SetPermission(Permission permission)
        {
            this.SetManyToOne(o => o.Permission, permission, o => o.RemoveRolePermission, o => o.RolePermissions);
        }

        public virtual void UnsetPermission()
        {
            this.UnsetManyToOne(o => o.Permission, o => o.RolePermissions);
        }

		#endregion

	}
}
