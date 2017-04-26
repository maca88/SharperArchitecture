using System;
using System.Collections.Generic;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;
using SharperArchitecture.Validation.Attributes;

namespace SharperArchitecture.Authentication.Entities
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
