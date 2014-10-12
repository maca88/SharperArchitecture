using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using FluentNHibernate.Automapping;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public partial class AttrUniqueAttribute : Entity
    {
        [Unique("FullName")]
        public virtual string FirstName { get; set; }

        [Unique("FullName")]
        public virtual string LastName { get; set; }

        [Unique("Random")]
        public virtual int Number { get; set; }

        [Unique("Random")]
        public virtual DateTime Date { get; set; }

        [Unique("Random")]
        public virtual string String { get; set; }

        [Unique("Year")]
        public virtual short Year { get; set; }

        [Unique]
        public virtual bool Alone { get; set; }

        [Unique]
        public virtual AttrUniqueAttribute Reference { get; set; }
    }
}
