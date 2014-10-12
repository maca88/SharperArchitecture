using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Entities
{
    public partial class UserRole : Entity
    {
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}
