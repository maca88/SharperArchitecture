using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Validation.Events
{
    public class ValidatorCreatedEvent : IEvent
    {
        public ValidatorCreatedEvent(IValidator validator, Type modelType)
        {
            Validator = validator;
            ModelType = modelType;
        }

        public IValidator Validator { get; }

        public Type ModelType { get; }
    }
}
