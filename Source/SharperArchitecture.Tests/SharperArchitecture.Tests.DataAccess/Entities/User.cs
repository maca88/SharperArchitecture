using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Attributes;

namespace SharperArchitecture.Tests.DataAccess.Entities
{
    public class User : VersionedEntity
    {
        public virtual string UserName { get; set; }
    }
}
