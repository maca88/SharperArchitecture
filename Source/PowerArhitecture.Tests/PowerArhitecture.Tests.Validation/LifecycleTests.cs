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
using Ninject;
using NUnit.Framework;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.Validation.Models;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.Validation
{
    [TestFixture]
    public class LifecycleTests : BaseTest
    {
        public LifecycleTests()
        {
            TestAssemblies.Add(typeof(Validator<>).Assembly);
        }

        [Test]
        public void ValidatorMustBeSingleton()
        {
            var validatorFactory = Kernel.Get<IValidatorFactory>();

            var validator = Kernel.Get<ITestModelValidator>();
            var validator2 = Kernel.Get<TestModelValidator>();
            var validator3 = Kernel.Get<IValidator<TestModel>>();
            var validator4 = Kernel.Get<IValidator<TestModel>>();
            var validator5 = validatorFactory.GetValidator(typeof(TestModel));
            var validator6 = validatorFactory.GetValidator<TestModel>();

            Assert.AreEqual(validator, validator2);
            Assert.AreEqual(validator2, validator3);
            Assert.AreEqual(validator3, validator4);
            Assert.AreEqual(validator4, validator5);
            Assert.AreEqual(validator5, validator6);
        }

        [Test]
        public void ValidatorEngineMustBeSingleton()
        {
            var validatorEngine = Kernel.Get<IValidatorEngine>();
            var validatorEngine2 = Kernel.Get<ValidatorEngine>();

            Assert.AreEqual(validatorEngine, validatorEngine2);
        }

        [Test]
        public void ValidatorFactoryMustBeSingleton()
        {
            var validatorFactory = Kernel.Get<IValidatorFactory>();
            var validatorFactory2 = Kernel.Get<IValidatorFactoryExtended>();

            Assert.AreEqual(validatorFactory, validatorFactory2);
        }

        [Test]
        public void ValidatorContextFillerMustBeTransient()
        {
            var contextFiller = Kernel.Get<IValidationContextFiller<TestModel>>();
            var contextFiller2 = Kernel.Get<IValidationContextFiller<TestModel>>();
            var contextFiller3 = Kernel.Get<ITestModelValidatonContextFiller>();

            Assert.AreEqual(typeof(TestModelValidatonContextFiller), contextFiller.GetType());
            Assert.AreEqual(typeof(TestModelValidatonContextFiller), contextFiller2.GetType());
            Assert.AreEqual(typeof(TestModelValidatonContextFiller), contextFiller3.GetType());
            Assert.AreNotEqual(contextFiller, contextFiller2);
            Assert.AreNotEqual(contextFiller, contextFiller3);
            Assert.AreNotEqual(contextFiller2, contextFiller3);
        }
    }

}
