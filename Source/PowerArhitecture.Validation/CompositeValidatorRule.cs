using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace PowerArhitecture.Validation
{/*
    public class CompositeValidatorRule : IValidationRule
    {
        private IValidator[] _validators;

        public CompositeValidatorRule(params IValidator[] validators)
        {
            _validators = validators;
        }

        #region IValidationRule Members
        public string RuleSet
        {
            get;
            set;
        }

        public IEnumerable<ValidationFailure> Validate(ValidationContext context)
        {
            var ret = new List<ValidationFailure>();

            foreach (var v in _validators)
            {
                ret.AddRange(v.Validate(context).Errors);
            }

            return ret;
        }

        public void ApplyCondition(Func<object, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IPropertyValidator> IValidationRule.Validators { get; private set; }

        IEnumerable<ValidationFailure> IValidationRule.Validate(ValidationContext context)
        {
            var ret = new List<ValidationFailure>();

            foreach (var v in _validators)
            {
                ret.AddRange(v.Validate(context).Errors);
            }

            return ret;
        }

        public IEnumerable<ServiceStack.FluentValidation.Validators.IPropertyValidator> Validators
        {
            get { yield break; }
        }
        #endregion
    }*/
}
