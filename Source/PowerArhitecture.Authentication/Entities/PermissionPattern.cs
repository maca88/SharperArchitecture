using System;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Authentication.Entities
{
    [Serializable]
    public partial class PermissionPattern : Entity
    {
        [NotNull]
        public virtual string Pattern { get; set; }

        [NotNull]
        public virtual Role Role { get; set; }
    }
}
