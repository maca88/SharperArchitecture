using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Data;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Tests.Notifications.Entities
{
    public class User : VersionedEntity
    {
        public virtual string UserName { get; set; }
    }
}
