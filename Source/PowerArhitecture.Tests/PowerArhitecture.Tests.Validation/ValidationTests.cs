using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NUnit.Framework;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.Validation.Models;
using PowerArhitecture.Validation;

namespace PowerArhitecture.Tests.Validation
{
    [TestFixture]
    public class ValidationTests : BaseTest
    {
        public ValidationTests()
        {
            TestAssemblies.Add(typeof(Validator<>).Assembly);
            TestAssemblies.Add(typeof(ValidationTests).Assembly);
        }

        [Test]
        public void ValidateWithContextFiller()
        {
            var validator = Container.GetInstance<IValidator<TestModel>>();
            var contextFiller = Container.GetInstance<ITestModelValidatonContextFiller>();
            var model = new TestModel();
            var valResult = validator.Validate(model, contextFiller: contextFiller);
            Assert.IsTrue(valResult.IsValid);
            Assert.AreEqual(1, contextFiller.FillCount);
            Assert.Throws<PowerArhitectureException>(() => validator.Validate(model));
        }

        [Test]
        public async Task ValidateWithContextFillerAsync()
        {
            var validator = Container.GetInstance<IValidator<TestModel>>();
            var contextFiller = Container.GetInstance<ITestModelValidatonContextFiller>();
            var model = new TestModel();
            var valResult = await validator.ValidateAsync(model, contextFiller: contextFiller);
            Assert.IsTrue(valResult.IsValid);
            Assert.AreEqual(1, contextFiller.FillCount);
            Assert.ThrowsAsync<PowerArhitectureException>(async () => await validator.ValidateAsync(model));
        }

        [Test]
        public void ValidateNesetdModelWithContextFiller()
        {
            var validator = Container.GetInstance<IValidator<Parent>>();
            var model = new Parent
            {
                Children = new List<Child>
                {
                    new Child
                    {
                        Name = "Child",
                        Children = new List<SubChild>
                        {
                            new SubChild {Name = "SubChild1"},
                            new SubChild()
                        }
                    },
                    new Child
                    {
                        Children = new List<SubChild>
                        {
                            new SubChild {Name = "SubChild1"}
                        }
                    }
                }
            };
            validator.Validate(model, contextFiller: null);

        }
    }
}
