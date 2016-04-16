using System.Collections.Generic;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace FluentValidation
{
    public static class IValidatorExtensions
    {
        public static void ValidateAndThrow(this IValidator validator, object instance,
            IEnumerable<string> ruleSets)
        {
            var valResult = Validate(validator, instance, string.Join(",", ruleSets));
            if (!valResult.IsValid)
                throw new ValidationException(valResult.Errors);
        }

        public static ValidationResult Validate(this IValidator validator, object instance, IEnumerable<string> ruleSets)
        {
            return Validate(validator, instance, string.Join(",", ruleSets));
        }

        public static void ValidateAndThrow(this IValidator validator, object instance, string ruleSets = null)
        {
            var valResult = Validate(validator, instance, ruleSets);
            if (!valResult.IsValid)
                throw new ValidationException(valResult.Errors);
        }

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
