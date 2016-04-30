using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Domain.Specifications;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Domain
{
    [Ignore]
    public abstract partial class VersionedCompositeEntity<TUser> : CompositeEntity, IVersionedEntityWithUser<TUser>
    {
        public virtual int Version { get; protected set; }

        public virtual DateTime CreatedDate { get; protected set; }

        public virtual DateTime? LastModifiedDate { get; protected set; }

        [NotNull]
        public virtual TUser CreatedBy { get; protected set; }

        public virtual TUser LastModifiedBy { get; protected set; }
    }

    [Ignore]
    public abstract partial class VersionedCompositeEntity<TUser, TType, TCol1, TCol2> : VersionedCompositeEntity<TUser>
    {
        protected abstract CompositeKey<TType, TCol1, TCol2> CreateCompositeKey();

        protected override ICompositeKey CreateCompositeKeyInternal()
        {
            return CreateCompositeKey();
        }
    }

    [Ignore]
    public abstract partial class VersionedCompositeEntity<TUser, TType, TCol1, TCol2, TCol3> : VersionedCompositeEntity<TUser>
    {
        protected abstract CompositeKey<TType, TCol1, TCol2, TCol3> CreateCompositeKey();

        protected override ICompositeKey CreateCompositeKeyInternal()
        {
            return CreateCompositeKey();
        }
    }

    [Ignore]
    public abstract partial class VersionedCompositeEntity<TUser, TType, TCol1, TCol2, TCol3, TCol4> : VersionedCompositeEntity<TUser>
    {
        protected abstract CompositeKey<TType, TCol1, TCol2, TCol3, TCol4> CreateCompositeKey();

        protected override ICompositeKey CreateCompositeKeyInternal()
        {
            return CreateCompositeKey();
        }
    }
}
