using System;

namespace PowerArhitecture.Domain.Specifications
{
    public interface IVersionedEntity<TUser> : IEntity
    {
        int Version { get; }

        DateTime CreatedDate { get; }

        TUser CreatedBy { get; }

        DateTime LastModifiedDate { get; }

        TUser LastModifiedBy { get; } 
    }
}
