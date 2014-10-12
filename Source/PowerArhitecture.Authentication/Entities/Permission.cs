using System;
using System.Collections.Generic;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Authentication.Entities
{
    [Serializable]
    public partial class Permission : Entity
    {
        [NotNull]
        [Unique("FullName")]
        public virtual string Name { get; set; }

        [Unique("FullName")]
        public virtual string Namespace { get; set; }

        [Unique("FullName")]
        [NotNull]
        public virtual string Module { get; set; }

        public virtual ISet<RolePermission> RolePermissions
        {
            get { return _rolePermissions ?? (_rolePermissions = new HashSet<RolePermission>()); }
            set { _rolePermissions = value; }
        }

        public virtual string FullName
        {
            get
            {
                return !string.IsNullOrEmpty(Namespace)
                    ? string.Format("{0}.{1}.{2}", Module, Namespace, Name)
                    : string.Format("{0}.{1}", Module, Name);
            }
        }
    }
}
