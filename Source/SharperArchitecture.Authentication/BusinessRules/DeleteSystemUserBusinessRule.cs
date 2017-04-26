using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Common.Internationalization;
using SharperArchitecture.Domain;
using SharperArchitecture.Validation;

namespace SharperArchitecture.Authentication.BusinessRules
{
    public class DeleteSystemUserBusinessRule<TUser> : AbstractBusinessRule<TUser>
        where TUser : class, IUser, IEntity<long>, new()
    {
        public override ValidationFailure Validate(TUser user, ValidationContext context)
        {
            return user.IsSystemUser ? Failure(I18N.Translate("System user cannot be deleted."), context) : null;
        }

        public override bool CanValidate(TUser user, ValidationContext context)
        {
            return true;
        }

        public override string[] RuleSets => new[] { ValidationRuleSet.Delete };
    }
}
