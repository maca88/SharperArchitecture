using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Authentication.Entities
{
    [Serializable]
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
