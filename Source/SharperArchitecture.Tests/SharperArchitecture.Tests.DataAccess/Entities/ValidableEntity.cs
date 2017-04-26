using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using NHibernate;
using NHibernate.Linq;
using SharperArchitecture.DataAccess.Extensions;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Specifications;
using SharperArchitecture.Validation;
using SharperArchitecture.Validation.Extensions;
using SharperArchitecture.Validation.Specifications;

namespace SharperArchitecture.Tests.DataAccess.Entities
{
    public class ValidableEntity : VersionedEntity, IAutoValidated
    {
        public virtual string Name { get; set; }

        public virtual bool ValidateOnUpdate => true;
        public virtual bool ValidateOnInsert => true;
        public virtual bool ValidateOnDelete => true;

        public virtual ISet<ValidableEntityChild> Children { get; set; } = new HashSet<ValidableEntityChild>();
    }


    public class ValidableEntityChild : Entity, IAggregateChild, IAutoValidated
    {
        public virtual ValidableEntity ValidableEntity { get; set; }

        public virtual bool ValidateOnUpdate => true;

        public virtual bool ValidateOnInsert => true;

        public virtual bool ValidateOnDelete => true;

        public virtual string CountryCode { get; set; }

        public virtual IVersionedEntity AggregateRoot => ValidableEntity;

        public virtual ISet<ValidableEntitySubChild> Children { get; set; } = new HashSet<ValidableEntitySubChild>();
    }

    public class ValidableEntitySubChild : Entity, IAggregateChild, IAutoValidated
    {
        public virtual ValidableEntityChild ValidableEntityChild { get; set; }

        public virtual bool ValidateOnUpdate => true;

        public virtual bool ValidateOnInsert => true;

        public virtual bool ValidateOnDelete => true;

        public virtual string Name { get; set; }

        public virtual IVersionedEntity AggregateRoot => ValidableEntityChild?.AggregateRoot;
    }

    public class ValidableEntityValidator : Validator<ValidableEntity>
    {
        public ValidableEntityValidator(IValidator<ValidableEntityChild> childValidator)
        {
            RuleSet(ValidationRuleSet.InsertUpdate, () =>
            {
                RuleFor(o => o.Children).SetCollectionValidator(childValidator);
                RuleFor(o => o.Name).NotEmpty();
            });
        }
    }

    public class ValidableEntityBusinessRule : AbstractBusinessRule<ValidableEntity>
    {
        public readonly ISession Session;

        public static int FilledCount { get; set; }

        public ValidableEntityBusinessRule(ISession session)
        {
            Session = session;
        }

        public override void BeforeValidation(ValidableEntity root, ValidationContext context)
        {
            FilledCount++;
            var codes = root.Children.Select(o => o.CountryCode).Distinct().ToList();
            context.RootContextData["validCodes"] =
                new HashSet<string>(Session.Query<Country>().Where(o => codes.Contains(o.Code)).Select(o => o.Code))
                {
                    "CannotUpdate"
                };
        }

        public override ValidationFailure Validate(ValidableEntity child, ValidationContext context)
        {
            return Success;
        }

        public override bool CanValidate(ValidableEntity child, ValidationContext context)
        {
            return true;
        }

        public override string[] RuleSets => ValidationRuleSet.InsertUpdate;
    }

    public class ValidableEntityChildValidator : Validator<ValidableEntityChild>
    {
        public ValidableEntityChildValidator(IValidator<ValidableEntitySubChild> subChildValidator)
        {
            RuleSet(ValidationRuleSet.InsertUpdate, () =>
            {
                RuleFor(o => o.Children).SetCollectionValidator(subChildValidator);
                RuleFor(o => o.CountryCode).Must((child, code, ctx) =>
                {
                    if (ctx.IsAutoValidating() && !ctx.IsRootAutoValidating())
                    {
                        return true;
                    }
                    var codes = (HashSet<string>) ctx.ParentContext.RootContextData["validCodes"];
                    return codes.Contains(code);
                }).WithL10NMessage("Code does not exist");
            });
            RuleSet(ValidationRuleSet.Delete, () =>
            {
                RuleFor(o => o.CountryCode).NotEqual("CannotDelete");
            });
            RuleSet(ValidationRuleSet.Insert, () =>
            {
                RuleFor(o => o.CountryCode).NotEqual("CannotInsert");
            });
            RuleSet(ValidationRuleSet.Update, () =>
            {
                RuleFor(o => o.CountryCode).NotEqual("CannotUpdate");
            });
        }
    }

    public class ValidableEntitySubChildValidator : Validator<ValidableEntitySubChild>
    {
        public ValidableEntitySubChildValidator()
        {
            RuleSet(ValidationRuleSet.InsertUpdate, () =>
            {
                RuleFor(o => o.Name).NotEmpty();
            });
            RuleSet(ValidationRuleSet.Delete, () =>
            {
                RuleFor(o => o.Name).NotEqual("CannotDelete");
            });
            RuleSet(ValidationRuleSet.Insert, () =>
            {
                RuleFor(o => o.Name).NotEqual("CannotInsert");
            });
        }
    }
}
