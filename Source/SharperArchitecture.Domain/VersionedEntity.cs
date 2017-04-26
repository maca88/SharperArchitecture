using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain.Attributes;
using SharperArchitecture.Domain.Specifications;

namespace SharperArchitecture.Domain
{
    [Serializable]
    [Ignore]
    public abstract class VersionedEntity : VersionedEntity<long>
    {
        public override bool IsTransient()
        {
            return Id <= 0; //Breeze will set this to a negative value
        }
    }

    [Serializable]
    [Ignore]
    public abstract class VersionedEntity<TId> :  Entity<TId>, IVersionedEntity<TId>
    {
        public virtual int Version { get; protected set; }

        public virtual DateTime CreatedDate { get; protected set; }

        public virtual DateTime? LastModifiedDate { get; protected set; }
    }
}
