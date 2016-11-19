using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NUnit.Framework;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.Validation.Models
{
    public class TestModel
    {
        public string Name { get; set; }
    }

    public interface ITestModelValidator : IValidator<TestModel>
    {
    }

    public class TestModelValidator : Validator<TestModel>, ITestModelValidator
    {
        public TestModelValidator()
        {
            Custom((model, context) =>
            {
                return !context.RootContextData.ContainsKey("Test")
                    ? Failure("RootContextData.Test")
                    : Success;
            });
        }
    }

    public interface ITestModelValidatonContextFiller : IValidationContextFiller<TestModel>
    {
        int FillCount { get; }
    }

    public class TestModelValidatonContextFiller : BaseValidationContextFiller<TestModel>, ITestModelValidatonContextFiller
    {
        public int FillCount { get; private set; }

        public override void FillContextData(TestModel model, Dictionary<string, object> contextData)
        {
            FillCount++;
            contextData["Test"] = true;
        }
    }
}
