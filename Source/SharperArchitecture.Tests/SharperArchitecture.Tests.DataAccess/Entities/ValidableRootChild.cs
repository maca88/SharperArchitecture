using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Specifications;
using SharperArchitecture.Validation.Attributes;

namespace SharperArchitecture.Tests.DataAccess.Entities
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
