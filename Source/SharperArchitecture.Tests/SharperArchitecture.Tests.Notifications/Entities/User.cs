using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Data;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Tests.Notifications.Entities
{
    public class User : VersionedEntity
    {
        public virtual string UserName { get; set; }
    }
}
