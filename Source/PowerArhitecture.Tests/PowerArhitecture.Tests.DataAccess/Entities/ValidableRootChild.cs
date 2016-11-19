using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public class ValidableRootChild : VersionedEntity, IAutoValidated, IAggregateChild
    {
        [NotNull]
        public virtual string Name { get; set; }

        public virtual ValidableRootChild Parent { get; set; }

        public virtual bool ValidateOnUpdate => true;

        public virtual bool ValidateOnInsert => true;

        public virtual bool ValidateOnDelete => true;

        public virtual IVersionedEntity AggregateRoot => Parent == null ? this : Parent.AggregateRoot;
    }
}
