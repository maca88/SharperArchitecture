using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NUnit.Framework;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.Validation.Models;
using PowerArhitecture.Tests.Validation.Models.Attributes;
using PowerArhitecture.Validation;

namespace PowerArhitecture.Tests.Validation
{
    [TestFixture]
    public class AttributeTests : BaseTest
    {
        public AttributeTests()
        {
            TestAssemblies.Add(typeof(Validator<>).Assembly);
        }

        [Test]
        public void Length()
        {
            var validator = Container.GetInstance<IValidator<LengthModel>>();

            var model = new LengthModel
            {
                Nickname = "Test",
                Nickname2 = "Test",
                Name = "Test",
                Name2 = "Test"
            };
            var result = validator.Validate(model, ruleSets: new [] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Nickname", result.Errors[0].PropertyName);
            Assert.AreEqual("Must be between 5 and 10 characters.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Nickname2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Nickname2' must be between 5 and 10 characters.", result.Errors[1].ErrorMessage);

            model = new LengthModel
            {
                Nickname = "Test2",
                Nickname2 = "Test2",
                Name = "Test012345679",
                Name2 = "Test012345679"
            };
            result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Name", result.Errors[0].PropertyName);
            Assert.AreEqual("Must be less than or equal to 10 characters.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Name2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Name2' must be less than or equal to 10 characters.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void NotNull()
        {
            var validator = Container.GetInstance<IValidator<NotNullModel>>();

            var model = new NotNullModel();
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Name", result.Errors[0].PropertyName);
            Assert.AreEqual("Is required.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Name2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Name2' is required.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void NotEmpty()
        {
            var validator = Container.GetInstance<IValidator<NotEmptyModel>>();

            var model = new NotEmptyModel
            {
                Name = "",
                Name2 = "",
                Name3 = "Default"
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(3, result.Errors.Count);
            Assert.AreEqual("Name", result.Errors[0].PropertyName);
            Assert.AreEqual("Should not be empty.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Name2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Name2' should not be empty.", result.Errors[1].ErrorMessage);
            Assert.AreEqual("Name3", result.Errors[2].PropertyName);
            Assert.AreEqual("'Name3' should not be empty.", result.Errors[2].ErrorMessage);
        }

        [Test]
        public void CreditCard()
        {
            var validator = Container.GetInstance<IValidator<CreditCardModel>>();

            var model = new CreditCardModel
            {
                CardNumber = "35ff54854",
                CardNumber2 = "3554f854"
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("CardNumber", result.Errors[0].PropertyName);
            Assert.AreEqual("Is not a valid credit card number.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("CardNumber2", result.Errors[1].PropertyName);
            Assert.AreEqual("'CardNumber2' is not a valid credit card number.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void Email()
        {
            var validator = Container.GetInstance<IValidator<EmailModel>>();

            var model = new EmailModel
            {
                Email = "invalid@email",
                Email2 = "invalid@email"
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Email", result.Errors[0].PropertyName);
            Assert.AreEqual("Is not a valid email address.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Email2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Email2' is not a valid email address.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void Equal()
        {
            var validator = Container.GetInstance<IValidator<EqualModel>>();

            var model = new EqualModel
            {
                Name = "Jon",
                Name2 = "Jon",
                LastName = "Ventura",
                LastName2 = "Ventura"
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(4, result.Errors.Count);
            Assert.AreEqual("Name", result.Errors[0].PropertyName);
            Assert.AreEqual("Should be equal to 'CompareValue'.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Name2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Name2' should be equal to 'CompareValue'.", result.Errors[1].ErrorMessage);
            Assert.AreEqual("LastName", result.Errors[2].PropertyName);
            Assert.AreEqual("Should be equal to 'LastNameCompare'.", result.Errors[2].ErrorMessage);
            Assert.AreEqual("LastName2", result.Errors[3].PropertyName);
            Assert.AreEqual("'LastName2' should be equal to 'LastNameCompare'.", result.Errors[3].ErrorMessage);
        }

        [Test]
        public void NotEqual()
        {
            var validator = Container.GetInstance<IValidator<NotEqualModel>>();

            var model = new NotEqualModel
            {
                Name = "CompareValue",
                Name2 = "CompareValue",
                LastName = "CompareProperty",
                LastName2 = "CompareProperty"
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(4, result.Errors.Count);
            Assert.AreEqual("Name", result.Errors[0].PropertyName);
            Assert.AreEqual("Should not be equal to 'CompareValue'.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Name2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Name2' should not be equal to 'CompareValue'.", result.Errors[1].ErrorMessage);
            Assert.AreEqual("LastName", result.Errors[2].PropertyName);
            Assert.AreEqual("Should not be equal to 'LastNameCompare'.", result.Errors[2].ErrorMessage);
            Assert.AreEqual("LastName2", result.Errors[3].PropertyName);
            Assert.AreEqual("'LastName2' should not be equal to 'LastNameCompare'.", result.Errors[3].ErrorMessage);
        }

        [Test]
        public void ExactLength()
        {
            var validator = Container.GetInstance<IValidator<ExactLengthModel>>();

            var model = new ExactLengthModel
            {
                Name = "Ann",
                Name2 = "Ann"
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Name", result.Errors[0].PropertyName);
            Assert.AreEqual("Must be 5 characters in length.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Name2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Name2' must be 5 characters in length.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void GreaterThan()
        {
            var validator = Container.GetInstance<IValidator<GreaterThanModel>>();

            var model = new GreaterThanModel
            {
                Value = 10,
                Value2 = 10
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Value", result.Errors[0].PropertyName);
            Assert.AreEqual("Must be greater than '10'.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Value2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Value2' must be greater than '10'.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void GreaterThanOrEqual()
        {
            var validator = Container.GetInstance<IValidator<GreaterThanOrEqualModel>>();

            var model = new GreaterThanOrEqualModel
            {
                Value = 9,
                Value2 = 9
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Value", result.Errors[0].PropertyName);
            Assert.AreEqual("Must be greater than or equal to '10'.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Value2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Value2' must be greater than or equal to '10'.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void LessThan()
        {
            var validator = Container.GetInstance<IValidator<LessThanModel>>();

            var model = new LessThanModel
            {
                Value = 10,
                Value2 = 10
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Value", result.Errors[0].PropertyName);
            Assert.AreEqual("Must be less than '10'.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Value2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Value2' must be less than '10'.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void LessThanOrEqual()
        {
            var validator = Container.GetInstance<IValidator<LessThanOrEqualModel>>();

            var model = new LessThanOrEqualModel
            {
                Value = 11,
                Value2 = 11
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Value", result.Errors[0].PropertyName);
            Assert.AreEqual("Must be less than or equal to '10'.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Value2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Value2' must be less than or equal to '10'.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void RegularExpression()
        {
            var validator = Container.GetInstance<IValidator<RegularExpressionModel>>();

            var model = new RegularExpressionModel
            {
                Name = "test",
                Name2 = "test"
            };
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual("Name", result.Errors[0].PropertyName);
            Assert.AreEqual("Is not in the correct format.", result.Errors[0].ErrorMessage);
            Assert.AreEqual("Name2", result.Errors[1].PropertyName);
            Assert.AreEqual("'Name2' is not in the correct format.", result.Errors[1].ErrorMessage);
        }

        [Test]
        public void IgnoreValidationAttributes()
        {
            var validator = Container.GetInstance<IValidator<IgnoreValidationAttributesModel>>();

            var model = new IgnoreValidationAttributesModel();
            var result = validator.Validate(model, ruleSets: new[] { ValidationRuleSet.Attribute });
            Assert.IsTrue(result.IsValid);
        }
    }
}
