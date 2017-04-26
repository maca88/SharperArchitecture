using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using SharperArchitecture.Validation.Specifications;

namespace SharperArchitecture.Validation
{
    public abstract class AbstractBusinessRule<TRoot> : AbstractBusinessRule<TRoot, TRoot>, IBusinessRule<TRoot>
        where TRoot : class
    { }

    public abstract class AbstractBusinessRule<TRoot, TChild> : IBusinessRule<TRoot, TChild>
        where TRoot : class
        where TChild : class
    {
        void IBusinessRule.BeforeValidation(object root, ValidationContext context)
        {
            BeforeValidation((TRoot)root, context);
        }

        ValidationFailure IBusinessRule.Validate(object child, ValidationContext context)
        {
            return Validate((TChild)child, context);
        }

        bool IBusinessRule.CanValidate(object child, ValidationContext context)
        {
            return CanValidate((TChild)child, context);
        }

        protected virtual ValidationFailure Failure(Expression<Func<TChild, object>> propertyExp, string errorMsg, ValidationContext context)
        {
            var child = context.InstanceToValidate as TChild;
            var attemptedValue = child != null ? propertyExp.Compile()(child) : null;
            return new ValidationFailure(propertyExp.GetFullPropertyName(), errorMsg, attemptedValue);
        }

        protected virtual ValidationFailure Failure(string errorMsg, ValidationContext context)
        {
            return new ValidationFailure(context.PropertyChain.ToString(), errorMsg, context.InstanceToValidate);
        }

        protected ValidationFailure Success => null;

        public virtual void BeforeValidation(TRoot root, ValidationContext context) { }

        public abstract ValidationFailure Validate(TChild child, ValidationContext context);

        public abstract bool CanValidate(TChild child, ValidationContext context);

        public abstract string[] RuleSets { get; }
    }
}
