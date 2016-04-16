using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Entities
{
    [Ignore]
    [Serializable]
    public abstract class Organization<TUser, TRole, TOrganization, TOrganizationRole> : VersionedEntityWithUser<TUser>, IOrganization<TUser>
        where TOrganization : Organization<TUser, TRole, TOrganization, TOrganizationRole>, new()
        where TOrganizationRole : OrganizationRole<TUser, TRole, TOrganization>, new()
        where TRole : IRole, IEntity, new()
        where TUser : IUser, IEntity, new()
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
    }
}
