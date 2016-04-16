using System;
using PowerArhitecture.Common.Attributes;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Validation.Attributes;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Entities
{
    [Ignore]
    [Serializable]
    public abstract class Role<TUser, TUserRole, TRole,  TRolePermission, TPermissionPattern> : VersionedEntityWithUser<TUser>, IRole
        where TPermissionPattern : PermissionPattern<TUser, TRole>, new()
        where TRolePermission : RolePermission<TUser, TRole>, new() 
        where TUser : IUser, IEntity, new()
        where TRole : Role<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern>, new()
        where TUserRole : UserRole<TUser, TRole>, new()
    {
        [Length(50)]
        [Unique]
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public void AddUser(IUser user)
        {
            AddUserRole(new TUserRole{User = (TUser)user});
        }

        public void RemoveUser(IUser user)
        {
            var ur = UserRoles.FirstOrDefault(o => o.UserId == user.Id);
            if (ur != null)
                RemoveUserRole(ur);
        }

        #region UserRoles

        private ISet<TUserRole> _userRoles;

        public virtual ISet<TUserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new HashSet<TUserRole>()); }
            set { _userRoles = value; }
        }

        public virtual void AddUserRole(TUserRole userRole)
        {
            this.AddOneToMany(o => o.UserRoles, userRole, o => o.Role, o => o.RemoveUserRole);
        }

        public virtual void RemoveUserRole(TUserRole userRole)
        {
            this.RemoveOneToMany(o => o.UserRoles, userRole, o => o.Role);
        }

		#endregion

		#region RolePermissions

        private ISet<TRolePermission> _rolePermissions;

        public virtual ISet<TRolePermission> RolePermissions
        {
            get { return _rolePermissions ?? (_rolePermissions = new HashSet<TRolePermission>()); }
            set { _rolePermissions = value; }
        }

        //public virtual void AddRolePermission(TRolePermission rolePermission)
        //{
        //    this.AddOneToMany(o => o.RolePermissions, rolePermission, o => o.Role, o=> o.RemoveRolePermission);
        //}

        //public virtual void RemoveRolePermission(TRolePermission rolePermission)
        //{
        //    this.RemoveOneToMany(o => o.RolePermissions, rolePermission, o => o.Role);
        //}

		#endregion

		#region PermissionPatterns

        private ISet<TPermissionPattern> _permissionPatterns;

        public virtual ISet<TPermissionPattern> PermissionPatterns
        {
            get { return _permissionPatterns ?? (_permissionPatterns = new HashSet<TPermissionPattern>()); }
            set { _permissionPatterns = value; }
        }

        //public virtual void AddPermissionPattern(PermissionPattern permissionPattern)
        //{
        //    this.AddOneToMany(o => o.PermissionPatterns, permissionPattern, o => o.Role, o=> o.RemovePermissionPattern);
        //}

        //public virtual void RemovePermissionPattern(PermissionPattern permissionPattern)
        //{
        //    this.RemoveOneToMany(o => o.PermissionPatterns, permissionPattern, o => o.Role);
        //}

		#endregion
    }
}
