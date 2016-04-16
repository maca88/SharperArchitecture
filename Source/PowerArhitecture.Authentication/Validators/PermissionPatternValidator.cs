using System.Text.RegularExpressions;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Internationalization;
using PowerArhitecture.Validation;
using FluentValidation.Results;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Validators
{
    public class PermissionPatternValidator: PAValidator<IPermissionPattern>
    {
        public PermissionPatternValidator()
        {
            RuleSet(ValidationRuleSet.InsertUpdate, () => Custom(AssertValidRegex));
        }

        private ValidationFailure AssertValidRegex(IPermissionPattern permission)
        {
            try
            {
                new Regex(permission.Pattern);
                return null;
            }
            catch
            {
                return ValidationFailure(o => o.Pattern, I18N.Translate("'{0}' is not in the correct format.", I18N.Translate("Pattern")));
            }
        }
    }
}
