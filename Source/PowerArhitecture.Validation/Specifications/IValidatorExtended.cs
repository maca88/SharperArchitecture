using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace PowerArhitecture.Validation.Specifications
{
    public interface IValidatorExtended : IValidator
    {
        bool HasValidationContextFiller { get; }

        bool CanValidateWithoutContextFiller { get; }

    }
}
