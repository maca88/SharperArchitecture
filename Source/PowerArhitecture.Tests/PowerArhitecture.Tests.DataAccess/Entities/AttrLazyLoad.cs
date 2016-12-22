using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using FluentNHibernate.Automapping;
using NHibernate;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Tests.DataAccess.MultiDatabase;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public partial class AttrLazyLoad : Entity
    {
        public virtual string Name { get; set; }

        [LazyLoad]
        public virtual string Xml { get; set; }
    }
}
