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
using NUnit.Framework;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.Validation.Models;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Factories;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.Validation
{
    [TestFixture]
    public class LifecycleTests : BaseTest
    {
        public LifecycleTests()
        {
            TestAssemblies.Add(typeof(Validator<>).Assembly);
            TestAssemblies.Add(typeof(LifecycleTests).Assembly);
        }

        [Test]
        public void ValidatorMustBeSingleton()
        {
            var validatorFactory = Container.GetInstance<IValidatorFactory>();

            var validator = Container.GetInstance<ITestModelValidator>();
            var validator2 = Container.GetInstance<TestModelValidator>();
            var validator3 = Container.GetInstance<IValidator<TestModel>>();
            var validator4 = Container.GetInstance<IValidator<TestModel>>();
            var validator5 = validatorFactory.GetValidator(typeof(TestModel));
            var validator6 = validatorFactory.GetValidator<TestModel>();
            var validator7 = Container.GetInstance<Validator<TestModel>>();
            var validator8 = Container.GetInstance<Validator<TestModel>>();

            Assert.AreEqual(validator, validator2);
            Assert.AreEqual(validator2, validator3);
            Assert.AreEqual(validator3, validator4);
            Assert.AreEqual(validator4, validator5);
            Assert.AreEqual(validator5, validator6);
            Assert.AreEqual(validator6, validator7);
            Assert.AreEqual(validator7, validator8);
        }

        [Test]
        public void GenericValidatorMustBeSingleton()
        {
            var validatorFactory = Container.GetInstance<IValidatorFactory>();

            var validator = Container.GetInstance<IValidator<SubChild>>();
            var validator2 = Container.GetInstance<Validator<SubChild>>();
            var validator3 = Container.GetInstance<Validator<SubChild>>();
            var validator4 = validatorFactory.GetValidator(typeof(SubChild));
            var validator5 = validatorFactory.GetValidator<SubChild>();

            Assert.AreEqual(typeof(Validator<SubChild>), validator.GetType());
            Assert.AreEqual(validator, validator2);
            Assert.AreEqual(validator2, validator3);
            Assert.AreEqual(validator3, validator4);
            Assert.AreEqual(validator4, validator5);
        }

        [Test]
        public void ValidatorFactoryMustBeSingleton()
        {
            var validatorFactory = Container.GetInstance<IValidatorFactory>();
            var validatorFactory2 = Container.GetInstance<IValidatorFactory>();

            Assert.AreEqual(validatorFactory, validatorFactory2);
        }

        [Test]
        public void ValidatorContextFillerMustBeTransient()
        {
            var contextFiller = Container.GetInstance<IValidationContextFiller<TestModel>>();
            var contextFiller2 = Container.GetInstance<IValidationContextFiller<TestModel>>();
            var contextFiller3 = Container.GetInstance<ITestModelValidatonContextFiller>();
            var contextFiller4 = Container.GetInstance<ITestModelValidatonContextFiller>();
            
            Assert.AreEqual(typeof(TestModelValidatonContextFiller), contextFiller.GetType());
            Assert.AreEqual(typeof(TestModelValidatonContextFiller), contextFiller2.GetType());
            Assert.AreEqual(typeof(TestModelValidatonContextFiller), contextFiller3.GetType());
            Assert.AreEqual(typeof(TestModelValidatonContextFiller), contextFiller4.GetType());
            Assert.AreNotEqual(contextFiller, contextFiller2);
            Assert.AreNotEqual(contextFiller, contextFiller3);
            Assert.AreNotEqual(contextFiller2, contextFiller3);
            Assert.AreNotEqual(contextFiller3, contextFiller4);
        }
    }

}
