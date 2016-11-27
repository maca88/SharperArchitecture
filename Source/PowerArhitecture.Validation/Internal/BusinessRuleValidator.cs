using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Validation.Internal
{
    internal class BusinessRuleValidator : IValidationRule
    {
        private readonly Func<ValidationContext, IEnumerable<IBusinessRule>> _rulesFunc;

        public BusinessRuleValidator(Func<ValidationContext, IEnumerable<IBusinessRule>> rulesFunc)
        {
            _rulesFunc = rulesFunc;
        }

        public IEnumerable<ValidationFailure> Validate(ValidationContext context)
        {
            foreach (var rule in _rulesFunc(context).Where(o => o.CanValidate(context.InstanceToValidate, context)))
            {
                var result = rule.Validate(context.InstanceToValidate, context);
                if (result == null)
                {
                    continue;
                }
                yield return result;
            }
        }

        public Task<IEnumerable<ValidationFailure>> ValidateAsync(ValidationContext context, CancellationToken cancellation)
        {
            var list = new List<ValidationFailure>();
            foreach (var rule in _rulesFunc(context).Where(o => o.CanValidate(context.InstanceToValidate, context)))
            {
                var result = rule.Validate(context.InstanceToValidate, context);
                if (result == null)
                {
                    continue;
                }
                list.Add(result);
            }
            return Task.FromResult<IEnumerable<ValidationFailure>>(list);
        }

        public void ApplyCondition(Func<object, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
        }

        public void ApplyAsyncCondition(Func<object, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
        }

        public IEnumerable<IPropertyValidator> Validators { get { yield break; } }

        public string RuleSet { get; set; }
    }
}
