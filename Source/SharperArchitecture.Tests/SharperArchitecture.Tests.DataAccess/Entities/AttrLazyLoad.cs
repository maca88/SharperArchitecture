using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;
using FluentNHibernate.Automapping;
using NHibernate;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Tests.DataAccess.MultiDatabase;

namespace SharperArchitecture.Tests.DataAccess.Entities
{
    public partial class AttrLazyLoad : Entity
    {
        public virtual string Name { get; set; }

        [LazyLoad]
        public virtual string Xml { get; set; }
    }
}
