using System.Collections.Generic;
using FluentValidation.Results;

namespace PowerArhitecture.Validation.Specifications
{
    public interface IValidatorEngine
    {
        ValidationResult Validate<T>(T model);

        ValidationResult Validate<T>(T model, string ruleSet);

        ValidationResult Validate(object model);

        ValidationResult Validate(object model, IEnumerable<string> ruleSets);

        ValidationResult Validate<T>(T model, IEnumerable<string> ruleSets);
    }
}