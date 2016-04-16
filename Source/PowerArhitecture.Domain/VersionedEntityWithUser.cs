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
    public abstract class VersionedEntityWithUser<TUser> : VersionedEntityWithUser<TUser, long>
    {
        public override bool IsTransient()
        {
            return Id <= 0; //Breeze will set this to a negative value
        }
    }

    [Serializable]
    [Ignore]
    public abstract class VersionedEntityWithUser<TUser, TType> : VersionedEntity<TType>, IVersionedEntityWithUser<TUser, TType>
    {
        [NotNull]
        public virtual TUser CreatedBy { get; protected set; }

        [NotNull]
        public virtual TUser LastModifiedBy { get; protected set; }

    }
}
