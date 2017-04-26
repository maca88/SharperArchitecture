using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace SharperArchitecture.Validation
{
    public class UniqueListValidator<TItem> : PropertyValidator
        where TItem : class
    {

        private readonly Expression<Func<TItem, object>> _propertyExpr;

        public UniqueListValidator(Expression<Func<TItem, object>> propertyExpr)
            : base("{SubProperty} must be unique")
        {
            _propertyExpr = propertyExpr;
        }

        public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context)
        {
            var fields = (IEnumerable<TItem>)context.PropertyValue;
            PropertyInfo propInfo = null;
            var propName = _propertyExpr.GetFullPropertyName();
            context.MessageFormatter.AppendArgument("SubProperty", propName);

            var currentItems = new HashSet<object>();
            var result = new List<ValidationFailure>();
            var idx = 0;
            foreach (var field in fields)
            {
                if (propInfo == null)
                    propInfo = field.GetPropertyInfo(_propertyExpr);
                var val = propInfo.GetValue(field);
                if (currentItems.Contains(val))
                {
                    var propertyChain = new PropertyChain();
                    propertyChain.Add(context.PropertyName);
                    propertyChain.AddIndexer(idx);
                    propertyChain.Add(propName);
                    
                    var error = context.MessageFormatter.BuildMessage(ErrorMessageSource.GetString(val));
                    result.Add(new ValidationFailure(propertyChain.ToString(), error));
                }   
                else
                    currentItems.Add(val);
                idx++;
            }
            return result;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            throw new NotSupportedException();
        }
    }
}
