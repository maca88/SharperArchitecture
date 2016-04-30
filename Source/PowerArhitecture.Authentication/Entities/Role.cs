using System;
using PowerArhitecture.Common.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
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
    public abstract partial class Role<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole> : VersionedEntityWithUser<TUser>, IRole
        where TUser : User<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TUserRole : UserRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRole : Role<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRolePermission : RolePermission<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TPermissionPattern : PermissionPattern<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganization : Organization<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganizationRole : OrganizationRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
    {
        [Length(50)]
        [Unique]
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual void AddUser(IUser user)
        {
            AddUserRole(new TUserRole { User = (TUser)user });
        }

        public virtual void RemoveUser(IUser user)
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

        #region LastModifiedBy

        [ReadOnly(true)]
        public virtual long? LastModifiedById
        {
            get
            {
                if (_lastModifiedByIdSet) return _lastModifiedById;
                return LastModifiedBy == null ? default(long?) : LastModifiedBy.Id;
            }
            set
            {
                _lastModifiedByIdSet = true;
                _lastModifiedById = value;
            }
        }

        private long? _lastModifiedById;

        private bool _lastModifiedByIdSet;

        #endregion

        #region CreatedBy

        [ReadOnly(true)]
        public virtual long? CreatedById
        {
            get
            {
                if (_createdByIdSet) return _createdById;
                return CreatedBy == null ? default(long?) : CreatedBy.Id;
            }
            set
            {
                _createdByIdSet = true;
                _createdById = value;
            }
        }

        private long? _createdById;

        private bool _createdByIdSet;

        #endregion
    }
}
