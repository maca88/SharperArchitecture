using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Common.Internationalization;
using SharperArchitecture.Domain;
using SharperArchitecture.Validation;

namespace SharperArchitecture.Authentication.BusinessRules
{
    public class UserNameFormatBusinessRule<TUser> : AbstractBusinessRule<TUser>
        where TUser : class, IUser, IEntity<long>, new()
    {
        public override ValidationFailure Validate(TUser user, ValidationContext context)
        {
            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                if (!Regex.IsMatch(user.UserName, "^[A-Za-z0-9]+$"))
                    return Failure(o => o.UserName, I18N.Translate("'{0}' is not in the correct format.",
                        I18N.Translate("UserName")), context);
            }
            else
                return Failure(o => o.UserName, I18N.Translate("'{0}' should not be empty.", I18N.Translate("UserName")), context);
            return Success;
        }

        public override bool CanValidate(TUser user, ValidationContext context)
        {
            return true;
        }

        public override string[] RuleSets => ValidationRuleSet.InsertUpdate;
    }
}
