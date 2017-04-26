using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using SharperArchitecture.Validation;
using SharperArchitecture.Validation.Specifications;

namespace SharperArchitecture.Tests.Validation.Models
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
    }

    public class TestModelBusinessRule : AbstractBusinessRule<TestModel>
    {
        public static int ValidateCount;
        public static int CanValidateCount;
        public static int ValidatBeforeValidationCount;

        public override void BeforeValidation(TestModel root, ValidationContext context)
        {
            ValidatBeforeValidationCount++;
        }

        public override ValidationFailure Validate(TestModel child, ValidationContext context)
        {
            ValidateCount++;
            return Success;
        }

        public override bool CanValidate(TestModel child, ValidationContext context)
        {
            CanValidateCount++;
            return true;
        }

        public override string[] RuleSets => new string[] {};
    }
}
