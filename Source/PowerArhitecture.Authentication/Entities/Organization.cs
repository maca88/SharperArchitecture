using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Entities
{
    public partial class Organization : VersionedEntity, IOrganization
    {
        public virtual ISet<OrganizationRole> OrganizationRoles
        {
            get { return _organizationRoles ?? (_organizationRoles = new HashSet<OrganizationRole>()); }
            set { _organizationRoles = value; }
        }
    }
}
