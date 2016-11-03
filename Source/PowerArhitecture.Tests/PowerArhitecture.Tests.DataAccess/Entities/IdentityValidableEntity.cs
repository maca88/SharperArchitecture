using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentValidation;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;
using PowerArhitecture.Validation;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public class IdentityValidableEntity : Entity, IAutoValidated
    {
        public virtual string Name { get; set; }

        public virtual bool ValidateOnUpdate => true;

        public virtual bool ValidateOnInsert => true;

        public virtual bool ValidateOnDelete => true;

        public virtual ISet<IdentityValidableChildEntity> Children { get; set; } = new HashSet<IdentityValidableChildEntity>();
    }

    public class IdentityValidableChildEntity : Entity, IAggregateChild
    {
        public virtual IdentityValidableEntity IdentityValidableEntity { get; set; }

        public virtual string Name { get; set; }

        public virtual object AggregateRoot => IdentityValidableEntity;
    }

    public class IdentityValidableEntityValidator : Validator<IdentityValidableEntity>
    {
        public IdentityValidableEntityValidator(IValidator<IdentityValidableChildEntity> childValidator)
        {
            RuleSet(ValidationRuleSet.InsertUpdate, () =>
            {
                RuleFor(o => o.Children).SetCollectionValidator(childValidator);
                RuleFor(o => o.Name).NotNull();
            });
        }
    }

    public class IdentityValidableChildEntityValidator : Validator<IdentityValidableChildEntity>
    {
        public IdentityValidableChildEntityValidator()
        {
            RuleSet(ValidationRuleSet.InsertUpdate, () =>
            {
                RuleFor(o => o.Name).NotNull();
            });

            RuleSet(ValidationRuleSet.Update, () =>
            {
                RuleFor(o => o.Name).NotEqual("CannotUpdate");
            });
            RuleSet(ValidationRuleSet.Delete, () =>
            {
                RuleFor(o => o.Name).NotEqual("CannotDelete");
            });
        }
    }


    public class IdentityValidableEntityOverride : IAutoMappingOverride<IdentityValidableEntity>
    {
        public void Override(AutoMapping<IdentityValidableEntity> mapping)
        {
            mapping.Id(o => o.Id).GeneratedBy.Identity();
            mapping.HasMany(o => o.Children).KeyColumn("IdentityValidableEntityId").Cascade.SaveUpdate();
        }
    }

    public class IdentityValidableChildEntityOverride : IAutoMappingOverride<IdentityValidableChildEntity>
    {
        public void Override(AutoMapping<IdentityValidableChildEntity> mapping)
        {
            mapping.Id(o => o.Id).GeneratedBy.Identity();
        }
    }
}
