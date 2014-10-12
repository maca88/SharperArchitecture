using System;
using System.ComponentModel;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Domain.Specifications;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Domain
{
    [Serializable]
    [Ignore]
    public class VersionedEntity : VersionedEntity<long, IUser> { }

    [Serializable]
    [Ignore]
    public partial class VersionedEntity<TType, TUser> : Entity<TType>, IVersionedEntity<TUser>
    {
        public virtual int Version { get; protected set; }

        public virtual DateTime CreatedDate { get; protected set; }

        [NotNull]
        public virtual TUser CreatedBy { get; protected set; }

        public virtual DateTime LastModifiedDate { get; protected set; }

        [NotNull]
        public virtual TUser LastModifiedBy { get; protected set; }
    }
}
