using System;
using SharperArchitecture.Domain.Attributes;
using SharperArchitecture.Domain.Specifications;

namespace SharperArchitecture.Domain
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
        public virtual TUser CreatedBy { get; protected set; }

        public virtual TUser LastModifiedBy { get; protected set; }

    }
}
