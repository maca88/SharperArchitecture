using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using FluentValidation;
using FluentValidation.Results;
using SharperArchitecture.Validation;

namespace SharperArchitecture.Tests.Validation.Models
{
    public class GenericChildParent
    {
        public List<GenericChildChild> Children { get; set; } = new List<GenericChildChild>();

        public GenericChildChild Relation { get; set; }
    }


    public abstract class GenericChildChild
    {
        public string Name { get; set; }
    }

    public class ConcreteGenericChildChild : GenericChildChild
    {
    }

    public class ConcreteGenericChildChild2 : GenericChildChild
    {
    }

    public class GenericChildParentValidator : Validator<GenericChildParent>
    {
        public GenericChildParentValidator(IValidator<GenericChildChild> childValidator)
        {
            RuleFor(o => o.Children).SetCollectionValidator(childValidator);
            RuleFor(o => o.Relation).SetValidator(childValidator);
        }
    }

    public class GenericChildBusinessRule<TChild> : AbstractBusinessRule<GenericChildParent, TChild> where TChild : GenericChildChild
    {
        public override ValidationFailure Validate(TChild child, ValidationContext context)
        {
            return string.IsNullOrEmpty(child.Name) ? Failure("Name should not be empty", context) :  Success;
        }

        public override bool CanValidate(TChild child, ValidationContext context)
        {
            return true;
        }

        public override string[] RuleSets { get; }
    }

}
