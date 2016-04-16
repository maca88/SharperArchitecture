using System;

namespace PowerArhitecture.Domain.Specifications
{
    public interface IVersionedEntity<out TId> : IEntity<TId>, IVersionedEntity
    {
    }

    public interface IVersionedEntity : IEntity
    {
        int Version { get; }

        DateTime CreatedDate { get; }

        DateTime? LastModifiedDate { get; }
    }
}
