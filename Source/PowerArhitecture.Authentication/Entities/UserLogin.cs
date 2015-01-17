using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Entities
{
    [Serializable]
    public partial class UserLogin : Entity
    {
        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }

        public virtual IUser User { get; set; }
    }
}
