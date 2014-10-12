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
	public partial class Role
	{

		#region UserRoles

		private ISet<UserRole> _userRoles;

        public virtual void AddUserRole(UserRole userRole)
        {
            this.AddOneToMany(o => o.UserRoles, userRole, o => o.Role, o=> o.RemoveUserRole);
        }

        public virtual void RemoveUserRole(UserRole userRole)
        {
            this.RemoveOneToMany(o => o.UserRoles, userRole, o => o.Role);
        }

		#endregion

		#region RolePermissions

		private ISet<RolePermission> _rolePermissions;

        public virtual void AddRolePermission(RolePermission rolePermission)
        {
            this.AddOneToMany(o => o.RolePermissions, rolePermission, o => o.Role, o=> o.RemoveRolePermission);
        }

        public virtual void RemoveRolePermission(RolePermission rolePermission)
        {
            this.RemoveOneToMany(o => o.RolePermissions, rolePermission, o => o.Role);
        }

		#endregion

		#region PermissionPatterns

		private ISet<PermissionPattern> _permissionPatterns;

        public virtual void AddPermissionPattern(PermissionPattern permissionPattern)
        {
            this.AddOneToMany(o => o.PermissionPatterns, permissionPattern, o => o.Role, o=> o.RemovePermissionPattern);
        }

        public virtual void RemovePermissionPattern(PermissionPattern permissionPattern)
        {
            this.RemoveOneToMany(o => o.PermissionPatterns, permissionPattern, o => o.Role);
        }

		#endregion

	}
}
