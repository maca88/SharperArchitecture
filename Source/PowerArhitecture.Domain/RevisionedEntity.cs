using System;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Domain.Specifications;
using NHibernate.Envers.Configuration.Attributes;

namespace PowerArhitecture.Domain
{
    [Serializable]
    [Ignore]
    public abstract class RevisionedEntity : RevisionedEntity<long, IUser>
    {
    }

    [Serializable]
    [Ignore]
    public abstract class RevisionedEntity<TId, TUser> : VersionedEntity<TId, TUser>, IRevisionedEntity
    {
        [RevisionNumber]
        public virtual int RevisionNumber { get; set; }

        [RevisionTimestamp]
        public virtual DateTime RevisionDate { get; set; }
    }
}
