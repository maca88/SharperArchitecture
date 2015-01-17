using System;
using System.Xml.Serialization;
using NHibernate.Envers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Domain.Specifications;
using NHibernate.Envers.Configuration.Attributes;

namespace PowerArhitecture.Domain
{
    [Serializable]
    [RevisionEntity]
    public class RevisionEntity : VersionedEntity<long, IUser>, IRevisionEntity
    {
        [RevisionNumber]
        [XmlIgnore]
        public override long Id { get; protected set; }

        [RevisionTimestamp]
        public virtual long RevisionTimestamp { get; set; }
    }
}
