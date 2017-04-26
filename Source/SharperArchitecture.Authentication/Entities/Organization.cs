using System;
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
    public abstract partial class Organization<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole> : VersionedEntityWithUser<TUser>, IOrganization<TUser>
        where TUser : User<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TUserRole : UserRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRole : Role<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRolePermission : RolePermission<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TPermissionPattern : PermissionPattern<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganization : Organization<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganizationRole : OrganizationRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
    {
        #region OrganizationRoles

        private ISet<TOrganizationRole> _organizationRoles;

        public virtual ISet<TOrganizationRole> OrganizationRoles
        {
            get { return _organizationRoles ?? (_organizationRoles = new HashSet<TOrganizationRole>()); }
            set { _organizationRoles = value; }
        }

        public virtual void AddOrganizationRole(TOrganizationRole organizationRole)
        {
            this.AddOneToMany(o => o.OrganizationRoles, organizationRole, o => o.Organization, o => o.RemoveOrganizationRole);
        }

        public virtual void RemoveOrganizationRole(TOrganizationRole organizationRole)
        {
            this.RemoveOneToMany(o => o.OrganizationRoles, organizationRole, o => o.Organization);
        }

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
