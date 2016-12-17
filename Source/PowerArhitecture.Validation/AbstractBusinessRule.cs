using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Validation
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

        protected ValidationFailure Failure(Expression<Func<TChild, object>> propertyExp, string errorMsg)
        {
            return new ValidationFailure(propertyExp.GetFullPropertyName(), errorMsg);
        }

        protected ValidationFailure Failure(string errorMsg)
        {
            return new ValidationFailure("", errorMsg);
        }

        protected ValidationFailure Success => null;

        public virtual void BeforeValidation(TRoot root, ValidationContext context) { }

        public abstract ValidationFailure Validate(TChild child, ValidationContext context);

        public abstract bool CanValidate(TChild child, ValidationContext context);

        public abstract string[] RuleSets { get; }
    }
}
