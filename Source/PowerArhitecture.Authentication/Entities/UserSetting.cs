using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Authentication.Entities
{
    public partial class UserSetting : Entity
    {
        public virtual IUser User { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        [Length(int.MaxValue)]
        public virtual string Value { get; set; }
    }
}
