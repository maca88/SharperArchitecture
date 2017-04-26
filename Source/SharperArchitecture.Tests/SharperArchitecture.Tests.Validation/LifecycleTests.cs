using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Validation.Attributes;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using NUnit.Framework;
using SharperArchitecture.Tests.Common;
using SharperArchitecture.Tests.Validation.Models;
using SharperArchitecture.Validation;
using SharperArchitecture.Validation.Internal;
using SharperArchitecture.Validation.Specifications;
using SimpleInjector;

namespace SharperArchitecture.Tests.Validation
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
            var validator = validatorFactory.GetValidator(typeof(TestModel));
            var validator2 = validatorFactory.GetValidator<TestModel>();
            var validator3 = Container.GetInstance<IValidator<TestModel>>();
            var validator4 = Container.GetInstance<IValidator<TestModel>>();
            
            Assert.AreEqual(validator, validator2);
            Assert.AreEqual(validator2, validator3);
            Assert.AreEqual(validator3, validator4);
        }

        [Test]
        public void CustomValidatorInterfaceShouldNotBeRegistered()
        {
            Assert.Throws<ActivationException>(() =>
            {
                Container.GetInstance<ITestModelValidator>();
            });
        }

        [Test]
        public void GenericValidatorMustBeSingleton()
        {
            var validatorFactory = Container.GetInstance<IValidatorFactory>();

            var validator = Container.GetInstance<IValidator<SubChild>>();
            var validator2 = Container.GetInstance<IValidator<SubChild>>();
            var validator3 = validatorFactory.GetValidator(typeof(SubChild));
            var validator4 = validatorFactory.GetValidator<SubChild>();

            Assert.AreEqual(typeof(ValidatorDecorator<SubChild>), validator.GetType());
            Assert.AreEqual(validator, validator2);
            Assert.AreEqual(validator2, validator3);
            Assert.AreEqual(validator3, validator4);
        }

        [Test]
        public void ValidatorFactoryMustBeSingleton()
        {
            var validatorFactory = Container.GetInstance<IValidatorFactory>();
            var validatorFactory2 = Container.GetInstance<IValidatorFactory>();

            Assert.AreEqual(validatorFactory, validatorFactory2);
        }
    }

}
