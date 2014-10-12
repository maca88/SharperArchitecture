using System;

namespace PowerArhitecture.Domain
{
    public interface IEntity<out TId> : IEntity
    {
        TId Id { get; }
    }

    public interface IEntity
    {
        bool IsTransient();

        object GetId();

        Type GetIdType();

        Type GetTypeUnproxied();
    }
}
