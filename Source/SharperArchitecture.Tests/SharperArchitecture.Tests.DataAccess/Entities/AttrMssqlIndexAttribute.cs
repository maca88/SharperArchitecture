using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;
using FluentNHibernate.Automapping;
using SharperArchitecture.Domain.Attributes.Mssql;

namespace SharperArchitecture.Tests.DataAccess.Entities
{
    public partial class AttrMssqlIndexAttribute : Entity
    {
        [MssqlIndex("Shared", Include = new []{ "NonIndexed1", "NonIndexed2" })]
        public virtual DateTime SharedIndex1 { get; set; }

        [MssqlIndex("Shared")]
        public virtual bool SharedIndex2 { get; set; }

        [MssqlIndex("Shared")]
        public virtual int SharedIndex3 { get; set; }

        [MssqlIndex]
        public virtual string Index1 { get; set; }

        [Index]
        public virtual AttrMssqlIndexAttribute Reference { get; set; }

        public virtual string NonIndexed1 { get; set; }

        public virtual string NonIndexed2 { get; set; }
    }
}
