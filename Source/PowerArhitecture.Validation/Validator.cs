using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Validation.Attributes;
using PowerArhitecture.Validation.Extensions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Validation
{
    /// <summary>
    /// The default validator if a custom one is not defined. This validator validates validation attributes
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class Validator<TModel> : AbstractValidator<TModel>, IValidatorExtended
    {
        public Validator()
        {
            AddAttributeValidation();
        }

        public bool HasValidationContextFiller { get; internal set; }

        public virtual bool CanValidateWithoutContextFiller => false;

        public override ValidationResult Validate(ValidationContext<TModel> context)
        {
            if (HasValidationContextFiller && !context.RootContextData.ContainsKey(ValidatorExtensions.DataContextFilledKey))
            {
                if (context.RootContextData.ContainsKey(ValidatorExtensions.DataContextFillerKey))
                {
                    var ctxFiller = context.RootContextData[ValidatorExtensions.DataContextFillerKey] as IValidationContextFiller<TModel>;
                    ctxFiller?.FillContextData(context.InstanceToValidate, context.RootContextData);
                }
                else if (!CanValidateWithoutContextFiller)
                {
                    throw new PowerArhitectureException($"Forbidden to validate model of type '{typeof(TModel).FullName}' without a validation context filler. " +
                                                        "Hint: override property CanValidateWithoutContextFiller or pass a context filler upon validation");
                }
                context.RootContextData[ValidatorExtensions.DataContextFilledKey] = true;
            }
            return base.Validate(context);
        }

        public override async Task<ValidationResult> ValidateAsync(ValidationContext<TModel> context, CancellationToken cancellation = new CancellationToken())
        {
            if (HasValidationContextFiller && !context.RootContextData.ContainsKey(ValidatorExtensions.DataContextFilledKey))
            {
                if (context.RootContextData.ContainsKey(ValidatorExtensions.DataContextFillerKey))
                {
                    var ctxFiller = context.RootContextData[ValidatorExtensions.DataContextFillerKey] as IValidationContextFiller<TModel>;
                    if (ctxFiller != null)
                    {
                        await ctxFiller.FillContextDataAsync(context.InstanceToValidate, context.RootContextData);
                    }
                }
                else if (!CanValidateWithoutContextFiller)
                {
                    throw new PowerArhitectureException($"Forbidden to validate model of type '{typeof(TModel).FullName}' without a validation context filler. " +
                                                        "Hint: override property CanValidateWithoutContextFiller or pass a context filler upon validation");
                }
                context.RootContextData[ValidatorExtensions.DataContextFilledKey] = true;
            }
            return await base.ValidateAsync(context, cancellation);
        }

        protected ValidationFailure Failure(Expression<Func<TModel, object>> propertyExp, string errorMsg)
        {
            return new ValidationFailure(propertyExp.GetFullPropertyName(), errorMsg);
        }

        protected ValidationFailure Failure(string errorMsg)
        {
            return new ValidationFailure("", errorMsg);
        }

        protected ValidationFailure Success => null;

        protected void RuleSet(IEnumerable<string> ruleSetNames, Action action)
        {
            foreach (var ruleSet in ruleSetNames)
            {
                RuleSet(ruleSet, action);
            }
        }

        protected virtual ValidationResult Validate(TModel model, string[] ruleSets)
        {
            var ruleSetSelector = ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory(ruleSets);
            return Validate(new ValidationContext<TModel>(model, new PropertyChain(), ruleSetSelector));
        }

        protected virtual Task<ValidationResult> ValidateAsync(TModel model, string[] ruleSets)
        {
            var ruleSetSelector = ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory(ruleSets);
            return ValidateAsync(new ValidationContext<TModel>(model, new PropertyChain(), ruleSetSelector));
        }


        private void AddAttributeValidation()
        {
            var type = typeof(TModel);

            var ignoreAttrsAttr = type.GetCustomAttributes(typeof (IgnoreValidationAttributesAttribute), true)
                .FirstOrDefault() as IgnoreValidationAttributesAttribute;
            var ignoreProps = ignoreAttrsAttr != null 
                ? new HashSet<string>(ignoreAttrsAttr.Properties ?? new string[0])
                : new HashSet<string>();

            foreach (var prop in type.GetProperties().Where(o => !ignoreProps.Contains(o.Name)))
            {
                var attrs = prop.GetCustomAttributes(true);
                //Add validation from attributes
                foreach (var attr in attrs.OfType<ValidationAttribute>())
                {
                    IPropertyValidator propValidator = null;
                    #region NotNullAttribute

                    if (attr is NotNullAttribute)
                    {
                        propValidator = new NotNullValidator();
                    }

                    #endregion
                    #region EmailAttribute

                    if (attr is EmailAttribute)
                    {
                        propValidator = new EmailValidator();
                    }

                    #endregion
                    #region NotEmptyAttribute

                    var notEmptyAttr = attr as NotEmptyAttribute;
                    if (notEmptyAttr != null)
                    {
                        propValidator = new NotEmptyValidator(notEmptyAttr.DefaultValue ?? prop.PropertyType.GetDefaultValue());
                    }

                    #endregion
                    #region EqualAttribute

                    AddComparisonValidator(attr as EqualAttribute, type, prop,
                        o => new EqualValidator(o), 
                        (func, info) => new EqualValidator(func, info));

                    #endregion
                    #region LengthAttribute

                    var lengthAttr = attr as LengthAttribute;
                    if (lengthAttr != null)
                    {
                        propValidator = new LengthValidator(lengthAttr.Min, lengthAttr.Max);
                    }

                    #endregion
                    #region NotEqualAttribute

                    AddComparisonValidator(attr as NotEqualAttribute, type, prop,
                        o => new NotEqualValidator(o),
                        (func, info) => new NotEqualValidator(func, info));

                    #endregion
                    #region RegularExpressionAttribute

                    var regexAttr = attr as RegularExpressionAttribute;
                    if (regexAttr != null)
                    {
                        propValidator = new RegularExpressionValidator(regexAttr.Expression, regexAttr.RegexOptions);
                    }

                    #endregion
                    #region CreditCardAttribute

                    if (attr is CreditCardAttribute)
                    {
                        propValidator = new CreditCardValidator();
                    }

                    #endregion
                    #region ExactLengthAttribute

                    var exctLenAttr = attr as ExactLengthAttribute;
                    if (exctLenAttr != null)
                    {
                        propValidator = new ExactLengthValidator(exctLenAttr.Length);
                    }

                    #endregion
                    #region GreaterThanAttribute

                    AddComparisonValidator(attr as GreaterThanAttribute, type, prop,
                        o => new GreaterThanValidator(o as IComparable),
                        (func, info) => new GreaterThanValidator(func, info));

                    #endregion
                    #region GreaterThanOrEqualAttribute

                    AddComparisonValidator(attr as GreaterThanOrEqualAttribute, type, prop,
                        o => new GreaterThanOrEqualValidator(o as IComparable),
                        (func, info) => new GreaterThanOrEqualValidator(func, info));

                    #endregion
                    #region LessThanOrEqualAttribute

                    AddComparisonValidator(attr as LessThanOrEqualAttribute, type, prop,
                        o => new LessThanOrEqualValidator(o as IComparable),
                        (func, info) => new LessThanOrEqualValidator(func, info));

                    #endregion
                    #region LessThanAttribute

                    AddComparisonValidator(attr as LessThanAttribute, type, prop,
                        o => new LessThanValidator(o as IComparable),
                        (func, info) => new LessThanValidator(func, info));

                    #endregion

                    if (propValidator == null)
                    {
                        continue;
                    }
                    AddAttributePropertyValidator(propValidator, prop, attr.IncludePropertyName);
                }
            }
        }

        private void AddComparisonValidator<T>(T attr, Type type, PropertyInfo prop, Func<object, IComparisonValidator> ctor1Fun,
            Func<Func<object, object>, MemberInfo, IComparisonValidator> ctor2Fun)
            where T: ComparisonAttribute
        {
            if(attr == null) return;
            IComparisonValidator propValidator = null;

            if (attr.CompareToValue != null)
            {
                propValidator = ctor1Fun(attr.CompareToValue);
            }
            if (attr.ComparsionProperty != null)
            {
                if (propValidator != null)
                    AddAttributePropertyValidator(propValidator, prop, attr.IncludePropertyName);

                var propInfo = type.GetProperty(attr.ComparsionProperty);
                if (propInfo == null)
                    throw new ArgumentException(string.Format("ComparsionProperty '{0}' of {1} in type '{2}' was not found",
                        attr.ComparsionProperty, typeof(T), type));

                Func<object, object> fun = propInfo.GetValue;
                propValidator = ctor2Fun(fun, propInfo);
            }
            AddAttributePropertyValidator(propValidator, prop, attr.IncludePropertyName);
        }

        private void AddAttributePropertyValidator(IPropertyValidator propValidator, PropertyInfo prop, bool includePropertyName)
        {
            AddPropertyValidator(propValidator, prop, ValidationRuleSet.Attribute, includePropertyName);
        }

        public void AddPropertyValidator(IPropertyValidator propValidator, PropertyInfo prop, string ruleSet, bool includePropertyName)
        {
            var rule = CreateRule(typeof(TModel), prop.Name);
            rule.RuleSet = ruleSet;
            rule.AddValidator(propValidator);
            rule.SetL10NMessage(includePropertyName);
            AddRule(rule);
        }

        private static PropertyRule CreateRule(Type type, string propName)
        {
            var p = Expression.Parameter(type);
            var body = Expression.Property(p, propName);
            var expr = Expression.Lambda(body, p);

            var propInfo = body.Member as PropertyInfo;
            if (propInfo == null)
                throw new NullReferenceException("propInfo");

            var createRuleMethod = typeof(PropertyRule)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Select(o => o.MakeGenericMethod(type, propInfo.PropertyType))
                .First(o => o.Name == "Create" && o.GetParameters().Count() == 1);
            return (PropertyRule)createRuleMethod.Invoke(null, new object[] { expr });
        }
    }
}
