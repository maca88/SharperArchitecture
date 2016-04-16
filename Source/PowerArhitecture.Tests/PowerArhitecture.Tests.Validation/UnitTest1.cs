using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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

        [TestMethod]
        public void KodaKamiona()
        {
            var totalTrucks = 5684;

            var rangeTrucks = Enumerable.Range(1000, 100);
            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(rangeTrucks, (trucks) =>
            {
                try
                {
                    IzracunjaKode(trucks);
                }
                catch (Exception exc)
                {
                    exceptions.Enqueue(exc);
                }
                
            });
            Assert.AreEqual("", string.Join(Environment.NewLine, exceptions.Select(o => o.Message)));
        }

        private static void IzracunjaKode(int stKamionov)
        {
            var rate = (int) (stKamionov/48);
            for (var j = 0; j < 1000; j++)
            {
                var unique = new HashSet<string>();
                var date = DateTime.Today;

                var curr = j;
                for (var i = 1; i < stKamionov; i++)
                {
                    if (i % rate == 0)
                    {
                        date = date.AddHours(1);
                    }

                    var code = KodaKamiona(curr, date);
                    if (unique.Contains(code))
                    {
                        throw new Exception("Trucks: " + stKamionov + " Rate /h: " + rate + " Unique values: " + unique.Count + " Code: " + code + " StartValue:" + j);
                    }
                    unique.Add(code);
                    curr++;
                }
            }
        }

        public static string KodaKamiona(int planPrihodaKamionovId, DateTime date)
        {
            var day = int.Parse(date.Day.ToString().Last().ToString());
            var datePart = (int)Math.Floor((date.Hour + 1) / 5m); // razpon med 0-4
            if (day % 2 == 1)
            {
                datePart += 5; // razpon 5-9
            }
            var stringNum = planPrihodaKamionovId.ToString().PadLeft(3, '0');
            stringNum = stringNum.Substring(stringNum.Length - 3);
            return new string(new[] { datePart.ToString()[0], stringNum[2], stringNum[1], stringNum[0] });
            var newNumber = Convert.ToInt32(new string(new[] { datePart.ToString()[0], stringNum[0], stringNum[1], stringNum[2] })) * 37 % 9999;
            return newNumber.ToString().PadLeft(4, '0');
        }
    }

}
