using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentValidation;
using FluentValidation.Results;
using NHibernate;
using NHibernate.Linq;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;
using PowerArhitecture.Validation;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public class UserExistance : Entity, IAutoValidated
    {
        public virtual string UserName { get; set; }

        public virtual bool ValidateOnUpdate => true;
        public virtual bool ValidateOnInsert => true;
        public virtual bool ValidateOnDelete => true;
    }

    public class UserExistanceOverride : IAutoMappingOverride<UserExistance>
    {
        public void Override(AutoMapping<UserExistance> mapping)
        {
            mapping.Id(o => o.Id).GeneratedBy.Identity();
        }
    }

    public class UserExistanceContextFiller : AbstractBusinessRule<UserExistance>
    {
        private readonly ISession _session;

        public UserExistanceContextFiller(ISession session)
        {
            _session = session;
        }

        public override void BeforeValidation(UserExistance root, ValidationContext context)
        {
            context.RootContextData["Session"] = _session;
        }

        public override ValidationFailure Validate(UserExistance child, ValidationContext context)
        {
            return Success;
        }

        public override bool CanValidate(UserExistance child, ValidationContext context)
        {
            return true;
        }

        public override string[] RuleSets { get; }
    }

    public class UserExistanceValidator : Validator<UserExistance>
    {
        public UserExistanceValidator()
        {
            RuleSet(ValidationRuleSet.Insert, () =>
            {
                Custom(CustomValidator);
            });
        }

        private ValidationFailure CustomValidator(UserExistance user, ValidationContext<UserExistance> ctx)
        {
            CurrentSession = (ISession) ctx.RootContextData["Session"];
            if (CurrentSession.Query<UserExistance>().Any(o => o.UserName == user.UserName))
            {
                return Failure("User already exists");
            }
            return Success;
        }

        public ISession CurrentSession { get; private set; }
    }
}
