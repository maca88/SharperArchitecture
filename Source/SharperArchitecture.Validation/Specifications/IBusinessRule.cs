﻿using System;
using FluentValidation;
using FluentValidation.Results;

namespace SharperArchitecture.Validation.Specifications
{
    public interface IBusinessRule
    {
        void BeforeValidation(object root, ValidationContext context);

        ValidationFailure Validate(object child, ValidationContext context);

        bool CanValidate(object child, ValidationContext context);

        string[] RuleSets { get; }
    }

    public interface IBusinessRule<TRoot> : IBusinessRule<TRoot, TRoot>
    {
    }

    public interface IBusinessRule<TRoot, TChild> : IBusinessRule
    {
        void BeforeValidation(TRoot root, ValidationContext context);

        ValidationFailure Validate(TChild child, ValidationContext context);

        bool CanValidate(TChild child, ValidationContext context);
    }
}
