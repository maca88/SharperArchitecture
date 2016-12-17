using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Attributes;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Tests.Validation.Models
{
    public class Parent
    {
        public List<Child> Children { get; set; } = new List<Child>();
    }

    public class ParentValidator : Validator<Parent>
    {
        public ParentValidator(IValidator<Child> childValidator)
        {
            RuleFor(o => o.Children).SetCollectionValidator(childValidator);
        }
    }

    public class Child
    {
        public string Name { get; set; }

        public List<SubChild> Children { get; set; } = new List<SubChild>();
    }

    public class ChildValidator : Validator<Child>
    {
        public ChildValidator(IValidator<SubChild> childValidator)
        {
            RuleFor(o => o.Children).SetCollectionValidator(childValidator);
        }
    }

    public class SubChild
    {
        [NotNull]
        public string Name { get; set; }
    }

    public class ParentBusinessRule : TestBusinessRule<Parent, Parent>
    { }

    public class ParentChildBusinessRule : TestBusinessRule<Parent, Child>
    { }

    public class ParentChild2BusinessRule : TestBusinessRule<Parent, Child, int>
    { }

    public class ParentSubChildBusinessRule : TestBusinessRule<Parent, SubChild>
    { }

    public class ChildSubChildBusinessRule : TestBusinessRule<Child, SubChild>
    { }

    public class SubChildBusinessRule : TestBusinessRule<SubChild, SubChild>
    { }

    public abstract class TestBusinessRule<TModel, TChild> : TestBusinessRule<TModel, TChild, byte>
        where TModel : class
        where TChild : class
    { }

    public abstract class TestBusinessRule<TModel, TChild, TType> : AbstractBusinessRule<TModel, TChild> 
        where TModel : class
        where TChild : class
    {
        public static List<IBusinessRule> Instances = new List<IBusinessRule>();
        public static List<Tuple<TChild, IBusinessRule>> ValidateModels = new List<Tuple<TChild, IBusinessRule>>();
        public static List<Tuple<TChild, IBusinessRule>> CanValidateModels = new List<Tuple<TChild, IBusinessRule>>();
        public static List<Tuple<TModel, IBusinessRule>> BeforeValidationModels = new List<Tuple<TModel, IBusinessRule>>();

        public TestBusinessRule()
        {
            Instances.Add(this);
        }

        public override void BeforeValidation(TModel root, ValidationContext context)
        {
            BeforeValidationModels.Add(new Tuple<TModel, IBusinessRule>(root, this));
        }

        public override ValidationFailure Validate(TChild child, ValidationContext context)
        {
            ValidateModels.Add(new Tuple<TChild, IBusinessRule>(child, this));
            return Success;
        }

        public override bool CanValidate(TChild child, ValidationContext context)
        {
            CanValidateModels.Add(new Tuple<TChild, IBusinessRule>(child, this));
            return true;
        }

        public override string[] RuleSets => new string[] {};

        public static void Clear()
        {
            Instances.Clear();
            ValidateModels.Clear();
            CanValidateModels.Clear();
            BeforeValidationModels.Clear();
        }
    }
}
