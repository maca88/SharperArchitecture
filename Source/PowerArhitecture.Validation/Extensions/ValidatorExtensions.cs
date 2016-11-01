using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Internal;
using FluentValidation.Results;
using PowerArhitecture.Validation.Specifications;

namespace FluentValidation
{
    public static class ValidatorExtensions
    {
        public const string DataContextFilledKey = "dataContextFilled";
        public const string DataContextFillerKey = "dataContextFiller";

        public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance, string[] ruleSets = null, 
            IValidationContextFiller<T> contextFiller = null)
        {
            var valResult = Validate(validator, instance, ruleSets, (object)contextFiller);
            if (!valResult.IsValid)
            {
                throw new ValidationException(valResult.Errors);
            }
        }

        public static async Task ValidateAndThrowAsync<T>(this IValidator<T> validator, T instance, string[] ruleSets = null,
            IValidationContextFiller<T> contextFiller = null)
        {
            var valResult = await ValidateAsync(validator, instance, ruleSets, (object)contextFiller);
            if (!valResult.IsValid)
            {
                throw new ValidationException(valResult.Errors);
            }
        }

        public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, string[] ruleSets = null,
            IValidationContextFiller<T> contextFiller = null)
        {
            return Validate(validator, instance, ruleSets, (object)contextFiller);
        }

        public static Task<ValidationResult> ValidateAsync<T>(this IValidator<T> validator, T instance, string[] ruleSets = null, 
            IValidationContextFiller<T> contextFiller = null)
        {
            return ValidateAsync(validator, instance, ruleSets, (object)contextFiller);
        }

        public static ValidationResult Validate(IValidator validator, object instance, string[] ruleSets, object contextFiller,
            Dictionary<string, object> extraData = null)
        {
            IValidatorSelector selector = new DefaultValidatorSelector();
            if (ruleSets != null)
            {
                selector = ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory(ruleSets);
            }
            var context = new ValidationContext(instance, new PropertyChain(), selector);
            if (contextFiller != null)
            {
                context.RootContextData[DataContextFillerKey] = contextFiller;
            }
            if (extraData != null)
            {
                foreach (var pair in extraData)
                {
                    context.RootContextData[pair.Key] = pair.Value;
                }
            }
            return validator.Validate(context);
        }

        public static Task<ValidationResult> ValidateAsync(IValidator validator, object instance, string[] ruleSets, object contextFiller)
        {
            IValidatorSelector selector = new DefaultValidatorSelector();
            if (ruleSets != null)
            {
                selector = ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory(ruleSets);
            }
            var context = new ValidationContext(instance, new PropertyChain(), selector);
            if (contextFiller != null)
            {
                context.RootContextData[DataContextFillerKey] = contextFiller;
            }
            return validator.ValidateAsync(context);
        }
    }
}
