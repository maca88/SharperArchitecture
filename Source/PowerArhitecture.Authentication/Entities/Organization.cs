using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Entities
{
    public partial class Organization : VersionedEntity
    {
        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual string FullName { get; set; }

        public virtual string Address { get; set; }

        public virtual ISet<OrganizationRole> OrganizationRoles
        {
            get { return _organizationRoles ?? (_organizationRoles = new HashSet<OrganizationRole>()); }
            set { _organizationRoles = value; }
        }
    }
}
