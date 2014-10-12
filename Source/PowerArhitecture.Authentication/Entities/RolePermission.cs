using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Entities
{
    public partial class RolePermission : Entity
    {
        public virtual Role Role { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
