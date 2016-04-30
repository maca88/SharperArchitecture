using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Entities
{
    [Ignore]
    [Serializable]
    public abstract partial class UserRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole> : Entity
        where TUser : User<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TUserRole : UserRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRole : Role<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRolePermission : RolePermission<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TPermissionPattern : PermissionPattern<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganization : Organization<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganizationRole : OrganizationRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
    {
        #region User

        [ReadOnly(true)]
        public virtual long? UserId
        {
            get
            {
                if (_userIdSet) return _userId;
                return User == null ? default(long?) : User.Id;
            }
            set
            {
                _userIdSet = true;
                _userId = value;
            }
        }

        private long? _userId;

        private bool _userIdSet;

        public virtual TUser User { get; set; }

        #endregion

        #region Role

        [ReadOnly(true)]
        public virtual long? RoleId
        {
            get
            {
                if (_roleIdSet) return _roleId;
                return Role == null ? default(long?) : Role.Id;
            }
            set
            {
                _roleIdSet = true;
                _roleId = value;
            }
        }

        private long? _roleId;

        private bool _roleIdSet;

        public virtual TRole Role { get; set; }

        #endregion

    }
}
