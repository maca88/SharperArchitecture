using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NHibernate;
using NHibernate.Linq;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Extensions;
using PowerArhitecture.Domain.Specifications;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Extensions;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public class ValidableEntity : Entity, IAutoValidated
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

        public virtual object AggregateRoot => ValidableEntity;

        public virtual ISet<ValidableEntitySubChild> Children { get; set; } = new HashSet<ValidableEntitySubChild>();
    }

    public class ValidableEntitySubChild : Entity, IAggregateChild, IAutoValidated
    {
        public virtual ValidableEntityChild ValidableEntityChild { get; set; }

        public virtual bool ValidateOnUpdate => true;

        public virtual bool ValidateOnInsert => true;

        public virtual bool ValidateOnDelete => true;

        public virtual string Name { get; set; }

        public virtual object AggregateRoot => ValidableEntityChild?.AggregateRoot;
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

    public class ValidableEntityValidationContextFiller : BaseValidationContextFiller<ValidableEntity>
    {
        private readonly ISession _session;

        public static int FilledCount { get; set; }

        public ValidableEntityValidationContextFiller(ISession session)
        {
            _session = session;
        }

        public override void FillContextData(ValidableEntity model, Dictionary<string, object> contextData)
        {
            FilledCount++;
            var codes = model.Children.Select(o => o.CountryCode).Distinct().ToList();
            contextData["validCodes"] =
                new HashSet<string>(_session.Query<Country>().Where(o => codes.Contains(o.Code)).Select(o => o.Code))
                {
                    "CannotUpdate"
                };
        }
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
