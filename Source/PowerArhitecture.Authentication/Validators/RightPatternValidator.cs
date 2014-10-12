using System.Text.RegularExpressions;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Internationalization;
using PowerArhitecture.Validation;
using FluentValidation.Results;

namespace PowerArhitecture.Authentication.Validators
{
    public class RightPatternValidator : PAValidator<PermissionPattern>
    {
        public RightPatternValidator()
        {
            RuleSet(ValidationRuleSet.InsertUpdate, () => Custom(AssertValidRegex));
        }

        private ValidationFailure AssertValidRegex(PermissionPattern permission)
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
