using System;
using System.Collections.Generic;
using FluentValidation;

namespace PowerArhitecture.Validation.Specifications
{
    public interface IValidatorFactoryExtended : IValidatorFactory
    {
        IDictionary<Type, IValidator> GetValidators(IEnumerable<Type> types);
    }
}