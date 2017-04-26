using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;
using FluentNHibernate.Automapping;

namespace SharperArchitecture.Tests.DataAccess.Entities
{
    public partial class AttrIndexAttribute : Entity
    {
        [Index("Shared")]
        public virtual DateTime SharedIndex1 { get; set; }

        [Index("Shared")]
        public virtual bool SharedIndex2 { get; set; }

        [Index("Shared")]
        public virtual int SharedIndex3 { get; set; }

        [Index]
        public virtual string Index1 { get; set; }

        [Index]
        public virtual AttrIndexAttribute Reference { get; set; }
    }
}
