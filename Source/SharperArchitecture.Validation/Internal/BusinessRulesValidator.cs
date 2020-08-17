using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using SharperArchitecture.Validation.Specifications;

namespace SharperArchitecture.Validation.Internal
{
    internal class BusinessRulesValidator : IValidationRule
    {
        private readonly Func<ValidationContext, IEnumerable<IBusinessRule>> _rulesFunc;

        public BusinessRulesValidator(Func<ValidationContext, IEnumerable<IBusinessRule>> rulesFunc)
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

        public void ApplyCondition(Func<ValidationContext, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
            throw new NotSupportedException();
        }

        public void ApplyAsyncCondition(Func<ValidationContext, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
            throw new NotSupportedException();
        }

        public void ApplyCondition(Func<PropertyValidatorContext, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
            throw new NotImplementedException();
        }

        public void ApplyAsyncCondition(Func<PropertyValidatorContext, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPropertyValidator> Validators { get { yield break; } }

        public string RuleSet { get; set; }
        public string[] RuleSets { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
