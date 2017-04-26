using System;
using System.ComponentModel;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;
using SharperArchitecture.Validation.Attributes;
using IRole = SharperArchitecture.Authentication.Specifications.IRole;

namespace SharperArchitecture.Authentication.Entities
{
    [Ignore]
    [Serializable]
    public abstract partial class PermissionPattern<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole> : Entity
        where TUser : User<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TUserRole : UserRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRole : Role<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRolePermission : RolePermission<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TPermissionPattern : PermissionPattern<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganization : Organization<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganizationRole : OrganizationRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
    {
        [NotNull]
        public virtual string Pattern { get; set; }

        #region Role

        [ReadOnly(true)]
        public virtual long RoleId
        {
            get
            {
                if (_roleIdSet) return _roleId;
                return Role?.Id ?? default(long);
            }
            set
            {
                _roleIdSet = true;
                _roleId = value;
            }
        }

        private long _roleId;

        private bool _roleIdSet;

        [NotNull]
        public virtual TRole Role { get; set; }

        #endregion

    }
}
