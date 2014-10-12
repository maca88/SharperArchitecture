using FluentValidation.Internal;
using FluentValidation.Results;

namespace FluentValidation
{
    public static class IValidatorExtensions
    {
        public static ValidationResult Validate(this IValidator validator, object instance, string ruleSets = null)
        {
            IValidatorSelector selector = new DefaultValidatorSelector();
			if(ruleSets != null) {
				var ruleSetNames = ruleSets.Split(',', ';');
				selector = new RulesetValidatorSelector(ruleSetNames);
			}
            var context = new ValidationContext(instance, new PropertyChain(), selector);
            return validator.Validate(context);
        }
    }
}
