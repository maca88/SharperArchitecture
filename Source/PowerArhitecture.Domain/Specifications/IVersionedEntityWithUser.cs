using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Domain.Specifications
{
    public interface IVersionedEntityWithUser<out TUser, out TId> : IVersionedEntityWithUser<TUser>, IVersionedEntity<TId>
    {
    }

    public interface IVersionedEntityWithUser<out TUser> : IVersionedEntity
    {
        TUser CreatedBy { get; }

        TUser LastModifiedBy { get; }
    }
}
