using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Validation.Attributes;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerArhitecture.Tests.Validation
{
    public class Language
    {
        [NotNull]
        [Length(20)]
        public string Culture { get; set; }

        public string Name { get; set; }
    }


    public class LanguageValidator : AbstractValidator<Language>
    {
        public LanguageValidator()
        {
            var type = typeof (Language);

            foreach (var prop in type.GetProperties())
            {
                var attrs = prop.GetCustomAttributes(true);
                foreach (var attr in attrs.OfType<Attribute>())
                {
                    IPropertyValidator propValidator = null;
                    string errorMsg = null;
                    if (attr is NotNullAttribute)
                    {
                        propValidator = new NotNullValidator();
                        errorMsg = "Required";
                    }
                        
                    var lengthAttr = attr as LengthAttribute;
                    if (lengthAttr != null)
                    {
                        errorMsg = "Length";
                        propValidator = new LengthValidator(lengthAttr.Min, lengthAttr.Max);
                    }
                        
                    if(propValidator == null) continue;

                    var rule = CreateRule(type, prop.Name);
                    rule.AddValidator(propValidator);
                    Func<PropertyValidatorContext, string> msgBuilder = context => errorMsg;
                    rule.MessageBuilder = msgBuilder;
                    AddRule(rule);
                }
            }

            


            /*
            rule.AddValidator(new NotNullValidator());
            Func<PropertyValidatorContext, string> msgBuilder = context => "Ktest";
            rule.MessageBuilder = msgBuilder;
            AddRule(rule);*/

            /*
            var rule = PropertyRule.Create<Language, object>(o => o.Culture);
            rule.AddValidator(new NotNullValidator());
            rule.MessageBuilder = context => "Kurac";
            AddRule(rule);*/


        }

        private PropertyRule CreateRule(Type type, string propName)
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

    [TestClass]
    public class Test32
    {
        [TestMethod]
        public void Test2()
        {
            var lang = new Language
            {
                Culture = "asdasd"
            };

            var validator = new LanguageValidator();
            validator.ValidateAndThrow(lang);
        }
    }

}
