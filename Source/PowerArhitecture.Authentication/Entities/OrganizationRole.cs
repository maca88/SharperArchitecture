using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Entities
{
    public partial class OrganizationRole : Entity
    {
        public virtual IOrganization Organization { get; set; }

        public virtual Role Role { get; set; }
    }
}
