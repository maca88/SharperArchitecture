﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;
using IRole = SharperArchitecture.Authentication.Specifications.IRole;

namespace SharperArchitecture.Authentication.Entities
{
    [Ignore]
    [Serializable]
    public abstract partial class OrganizationRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole> : Entity
        where TUser : User<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TUserRole : UserRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRole : Role<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRolePermission : RolePermission<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TPermissionPattern : PermissionPattern<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganization : Organization<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganizationRole : OrganizationRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
    {
        #region Organization

        [ReadOnly(true)]
        public virtual long? OrganizationId
        {
            get
            {
                if (_organizationIdSet) return _organizationId;
                return Organization == null ? default(long?) : Organization.Id;
            }
            set
            {
                _organizationIdSet = true;
                _organizationId = value;
            }
        }

        private long? _organizationId;

        private bool _organizationIdSet;

        public virtual TOrganization Organization { get; set; }

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
