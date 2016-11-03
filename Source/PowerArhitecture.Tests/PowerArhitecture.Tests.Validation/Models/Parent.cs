using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using FluentValidation;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Attributes;
using PowerArhitecture.Validation.Extensions;

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
}
