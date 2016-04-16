using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public class User : VersionedEntity
    {
        public virtual string UserName { get; set; }
    }
}
