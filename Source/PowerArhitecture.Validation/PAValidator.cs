using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ninject;
using PowerArhitecture.Validation.Attributes;
using PowerArhitecture.Validation.Extensions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace PowerArhitecture.Validation
{
    //the default validator if a custom one is not defined. This validator validates validation attributes
    public class PAValidator<TModel> : AbstractValidator<TModel>
    {
        public PAValidator()
        {
            AddAttributeValidation();
        }

        protected ValidationFailure ValidationFailure(Expression<Func<TModel, object>> propertyExp,
                                                                 string errorMsg)
        {
            return new ValidationFailure(propertyExp.GetFullPropertyName(), errorMsg);
            
        }

        protected ValidationFailure ValidationFailure(string errorMsg)
        {
            return new ValidationFailure("", errorMsg);
        }

        protected ValidationFailure ValidationSuccess
        {
            get
            {
                return null;
            }
        }

        protected void RuleSet(IEnumerable<string> ruleSetNames, Action action)
        {
            foreach (var ruleSet in ruleSetNames)
            {
                RuleSet(ruleSet, action);
            }
        }

        protected ValidationResult Validate(TModel model, IEnumerable<string> ruleSets)
        {
            var ruleSetSelector = new PARulesetValidatorSelector(ruleSets);
            return Validate(new ValidationContext<TModel>(model, new PropertyChain(), ruleSetSelector));
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

                    if (attr is NotEmptyAttribute)
                    {
                        propValidator = new NotEmptyValidator(prop.PropertyType.GetDefaultValue());
                    }

                    #endregion
                    #region EqualAttribute

                    AddComparisonValidator(attr as EqualAttribute, type, prop,
                        o => new EqualValidator(o), 
                        (func, info) => new EqualValidator(func, info));

                    #endregion
                    #region ExclusiveBetweenAttribute

                    var exclBtw = attr as ExclusiveBetweenAttribute;
                    if (exclBtw != null)
                    {
                        propValidator = new ExclusiveBetweenValidator(exclBtw.From, exclBtw.To);
                    }

                    #endregion
                    #region InclusiveBetweenAttribute

                    var inclBtw = attr as InclusiveBetweenAttribute;
                    if (inclBtw != null)
                    {
                        propValidator = new InclusiveBetweenValidator(inclBtw.From, inclBtw.To);
                    }

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
                    if (propValidator == null) continue;
                    AddPropertyValidator(propValidator, type, prop, attr);
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
                    AddPropertyValidator(propValidator, type, prop, attr);

                var propInfo = type.GetProperty(attr.ComparsionProperty);
                if (propInfo == null)
                    throw new ArgumentException(string.Format("ComparsionProperty '{0}' of {1} in type '{2}' was not found",
                        attr.ComparsionProperty, typeof(T), type));

                Func<object, object> fun = propInfo.GetValue;
                propValidator = ctor2Fun(fun, propInfo);
            }
            AddPropertyValidator(propValidator, type, prop, attr);
        }

        private void AddPropertyValidator(IPropertyValidator propValidator, Type type, PropertyInfo prop, ValidationAttribute valAttr)
        {
            var rule = CreateRule(type, prop.Name);
            //rule.RuleSet = ruleSet; //default is for client side (no need to specify ruleset in controller action)
            rule.RuleSet = ValidationRuleSet.Attribute;
            rule.AddValidator(propValidator);
            rule.SetL10NMessage(valAttr.IncludePropertyName);
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
