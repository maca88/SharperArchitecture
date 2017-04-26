using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using SharperArchitecture.Tests.Common;
using SharperArchitecture.Tests.Validation.Models;
using SharperArchitecture.Validation;

namespace SharperArchitecture.Tests.Validation
{
    [TestFixture]
    public class ValidationTests : BaseTest
    {
        public ValidationTests()
        {
            TestAssemblies.Add(typeof(Validator<>).Assembly);
            TestAssemblies.Add(typeof(ValidationTests).Assembly);
        }

        [Test]
        public void ValidateWithBusinessRule()
        {
            TestModelBusinessRule.ValidateCount = 0;
            TestModelBusinessRule.ValidatBeforeValidationCount = 0;
            var validator = Container.GetInstance<IValidator<TestModel>>();

            for (var i = 0; i < 5; i++)
            {
                var model = new TestModel();
                var valResult = validator.Validate(model);
                Assert.IsTrue(valResult.IsValid);
                Assert.AreEqual(i + 1, TestModelBusinessRule.ValidateCount);
                Assert.AreEqual(i + 1, TestModelBusinessRule.CanValidateCount);
                Assert.AreEqual(i + 1, TestModelBusinessRule.ValidatBeforeValidationCount);
            }
        }

        [Test]
        public void ValidateNestedModelWithBusinessRule()
        {
            var valFuns = new List<Func<IValidator<Parent>, Parent, ValidationResult>>
            {
                (v, m) => v.Validate(m),
                (v, m) => v.Validate((object) m),
                (v, m) => v.Validate(new ValidationContext(m)),
                (v, m) => v.Validate(new ValidationContext<Parent>(m)),
                (v, m) => v.Validate(m, ruleSet: ValidationRuleSet.Default),
                (v, m) => v.Validate(m, ruleSets: new[] {ValidationRuleSet.Default})
            };

            foreach (var valFun in valFuns)
            {
                for (var i = 0; i < 5; i++)
                {
                    ParentBusinessRule.Clear();
                    ParentChildBusinessRule.Clear();
                    ParentChild2BusinessRule.Clear();
                    ChildSubChildBusinessRule.Clear();
                    ParentSubChildBusinessRule.Clear();
                    SubChildBusinessRule.Clear();

                    var validator = Container.GetInstance<IValidator<Parent>>();
                    var model = new Parent
                    {
                        Children = new List<Child>
                        {
                            new Child
                            {
                                Name = "Child",
                                Children = new List<SubChild>
                                {
                                    new SubChild {Name = "SubChild1"},
                                    new SubChild()
                                }
                            },
                            new Child
                            {
                                Children = new List<SubChild>
                                {
                                    new SubChild {Name = "SubChild1"}
                                }
                            }
                        }
                    };
                    var result = valFun(validator, model);

                    Assert.IsTrue(result.IsValid);

                    Assert.AreEqual(1, ParentBusinessRule.Instances.Count);
                    Assert.AreEqual(1, ParentBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model, ParentBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(1, ParentBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model, ParentBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(1, ParentBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model, ParentBusinessRule.BeforeValidationModels[0].Item1);

                    Assert.AreEqual(1, ParentChildBusinessRule.Instances.Count);
                    Assert.AreEqual(2, ParentChildBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0], ParentChildBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ParentChildBusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(2, ParentChildBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0], ParentChildBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ParentChildBusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(1, ParentChildBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model, ParentChildBusinessRule.BeforeValidationModels[0].Item1);

                    Assert.AreEqual(1, ParentChild2BusinessRule.Instances.Count);
                    Assert.AreEqual(2, ParentChild2BusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0], ParentChild2BusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ParentChild2BusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(2, ParentChild2BusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0], ParentChild2BusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ParentChild2BusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(1, ParentChild2BusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model, ParentChild2BusinessRule.BeforeValidationModels[0].Item1);

                    Assert.AreEqual(1, ParentSubChildBusinessRule.Instances.Count);
                    Assert.AreEqual(3, ParentSubChildBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], ParentSubChildBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], ParentSubChildBusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], ParentSubChildBusinessRule.ValidateModels[2].Item1);
                    Assert.AreEqual(3, ParentSubChildBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], ParentSubChildBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], ParentSubChildBusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], ParentSubChildBusinessRule.CanValidateModels[2].Item1);
                    Assert.AreEqual(1, ParentSubChildBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model, ParentSubChildBusinessRule.BeforeValidationModels[0].Item1);

                    Assert.AreEqual(2, ChildSubChildBusinessRule.Instances.Count);
                    Assert.AreEqual(3, ChildSubChildBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], ChildSubChildBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], ChildSubChildBusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], ChildSubChildBusinessRule.ValidateModels[2].Item1);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.ValidateModels[0].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.ValidateModels[1].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[1], ChildSubChildBusinessRule.ValidateModels[2].Item2);
                    Assert.AreEqual(3, ChildSubChildBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], ChildSubChildBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], ChildSubChildBusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], ChildSubChildBusinessRule.CanValidateModels[2].Item1);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.CanValidateModels[0].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.CanValidateModels[1].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[1], ChildSubChildBusinessRule.CanValidateModels[2].Item2);
                    Assert.AreEqual(2, ChildSubChildBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model.Children[0], ChildSubChildBusinessRule.BeforeValidationModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ChildSubChildBusinessRule.BeforeValidationModels[1].Item1);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.BeforeValidationModels[0].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[1], ChildSubChildBusinessRule.BeforeValidationModels[1].Item2);

                    Assert.AreEqual(3, SubChildBusinessRule.Instances.Count);
                    Assert.AreEqual(3, SubChildBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], SubChildBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], SubChildBusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], SubChildBusinessRule.ValidateModels[2].Item1);
                    Assert.AreEqual(SubChildBusinessRule.Instances[0], SubChildBusinessRule.ValidateModels[0].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[1], SubChildBusinessRule.ValidateModels[1].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[2], SubChildBusinessRule.ValidateModels[2].Item2);
                    Assert.AreEqual(3, SubChildBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], SubChildBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], SubChildBusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], SubChildBusinessRule.CanValidateModels[2].Item1);
                    Assert.AreEqual(SubChildBusinessRule.Instances[0], SubChildBusinessRule.CanValidateModels[0].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[1], SubChildBusinessRule.CanValidateModels[1].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[2], SubChildBusinessRule.CanValidateModels[2].Item2);
                    Assert.AreEqual(3, SubChildBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], SubChildBusinessRule.BeforeValidationModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], SubChildBusinessRule.BeforeValidationModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], SubChildBusinessRule.BeforeValidationModels[2].Item1);
                    Assert.AreEqual(SubChildBusinessRule.Instances[0], SubChildBusinessRule.BeforeValidationModels[0].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[1], SubChildBusinessRule.BeforeValidationModels[1].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[2], SubChildBusinessRule.BeforeValidationModels[2].Item2);
                }
            }
        }

        [Test]
        public async Task ValidateNestedModelWithBusinessRuleAsync()
        {
            var valFuns = new List<Func<IValidator<Parent>, Parent, Task<ValidationResult>>>
            {
                (v, m) => v.ValidateAsync(m),
                (v, m) => v.ValidateAsync((object) m),
                (v, m) => v.ValidateAsync(new ValidationContext(m)),
                (v, m) => v.ValidateAsync(new ValidationContext<Parent>(m)),
                (v, m) => v.ValidateAsync(m, ruleSet: ValidationRuleSet.Default),
                (v, m) => v.ValidateAsync(m, ruleSets: new[] {ValidationRuleSet.Default})
            };

            foreach (var valFun in valFuns)
            {
                for (var i = 0; i < 5; i++)
                {
                    ParentBusinessRule.Clear();
                    ParentChildBusinessRule.Clear();
                    ParentChild2BusinessRule.Clear();
                    ChildSubChildBusinessRule.Clear();
                    ParentSubChildBusinessRule.Clear();
                    SubChildBusinessRule.Clear();

                    var validator = Container.GetInstance<IValidator<Parent>>();
                    var model = new Parent
                    {
                        Children = new List<Child>
                        {
                            new Child
                            {
                                Name = "Child",
                                Children = new List<SubChild>
                                {
                                    new SubChild {Name = "SubChild1"},
                                    new SubChild()
                                }
                            },
                            new Child
                            {
                                Children = new List<SubChild>
                                {
                                    new SubChild {Name = "SubChild1"}
                                }
                            }
                        }
                    };
                    var result = await valFun(validator, model);

                    Assert.IsTrue(result.IsValid);

                    Assert.AreEqual(1, ParentBusinessRule.Instances.Count);
                    Assert.AreEqual(1, ParentBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model, ParentBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(1, ParentBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model, ParentBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(1, ParentBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model, ParentBusinessRule.BeforeValidationModels[0].Item1);

                    Assert.AreEqual(1, ParentChildBusinessRule.Instances.Count);
                    Assert.AreEqual(2, ParentChildBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0], ParentChildBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ParentChildBusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(2, ParentChildBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0], ParentChildBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ParentChildBusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(1, ParentChildBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model, ParentChildBusinessRule.BeforeValidationModels[0].Item1);

                    Assert.AreEqual(1, ParentChild2BusinessRule.Instances.Count);
                    Assert.AreEqual(2, ParentChild2BusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0], ParentChild2BusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ParentChild2BusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(2, ParentChild2BusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0], ParentChild2BusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ParentChild2BusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(1, ParentChild2BusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model, ParentChild2BusinessRule.BeforeValidationModels[0].Item1);

                    Assert.AreEqual(1, ParentSubChildBusinessRule.Instances.Count);
                    Assert.AreEqual(3, ParentSubChildBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], ParentSubChildBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], ParentSubChildBusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], ParentSubChildBusinessRule.ValidateModels[2].Item1);
                    Assert.AreEqual(3, ParentSubChildBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], ParentSubChildBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], ParentSubChildBusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], ParentSubChildBusinessRule.CanValidateModels[2].Item1);
                    Assert.AreEqual(1, ParentSubChildBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model, ParentSubChildBusinessRule.BeforeValidationModels[0].Item1);

                    Assert.AreEqual(2, ChildSubChildBusinessRule.Instances.Count);
                    Assert.AreEqual(3, ChildSubChildBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], ChildSubChildBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], ChildSubChildBusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], ChildSubChildBusinessRule.ValidateModels[2].Item1);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.ValidateModels[0].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.ValidateModels[1].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[1], ChildSubChildBusinessRule.ValidateModels[2].Item2);
                    Assert.AreEqual(3, ChildSubChildBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], ChildSubChildBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], ChildSubChildBusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], ChildSubChildBusinessRule.CanValidateModels[2].Item1);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.CanValidateModels[0].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.CanValidateModels[1].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[1], ChildSubChildBusinessRule.CanValidateModels[2].Item2);
                    Assert.AreEqual(2, ChildSubChildBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model.Children[0], ChildSubChildBusinessRule.BeforeValidationModels[0].Item1);
                    Assert.AreEqual(model.Children[1], ChildSubChildBusinessRule.BeforeValidationModels[1].Item1);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[0], ChildSubChildBusinessRule.BeforeValidationModels[0].Item2);
                    Assert.AreEqual(ChildSubChildBusinessRule.Instances[1], ChildSubChildBusinessRule.BeforeValidationModels[1].Item2);

                    Assert.AreEqual(3, SubChildBusinessRule.Instances.Count);
                    Assert.AreEqual(3, SubChildBusinessRule.ValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], SubChildBusinessRule.ValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], SubChildBusinessRule.ValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], SubChildBusinessRule.ValidateModels[2].Item1);
                    Assert.AreEqual(SubChildBusinessRule.Instances[0], SubChildBusinessRule.ValidateModels[0].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[1], SubChildBusinessRule.ValidateModels[1].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[2], SubChildBusinessRule.ValidateModels[2].Item2);
                    Assert.AreEqual(3, SubChildBusinessRule.CanValidateModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], SubChildBusinessRule.CanValidateModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], SubChildBusinessRule.CanValidateModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], SubChildBusinessRule.CanValidateModels[2].Item1);
                    Assert.AreEqual(SubChildBusinessRule.Instances[0], SubChildBusinessRule.CanValidateModels[0].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[1], SubChildBusinessRule.CanValidateModels[1].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[2], SubChildBusinessRule.CanValidateModels[2].Item2);
                    Assert.AreEqual(3, SubChildBusinessRule.BeforeValidationModels.Count);
                    Assert.AreEqual(model.Children[0].Children[0], SubChildBusinessRule.BeforeValidationModels[0].Item1);
                    Assert.AreEqual(model.Children[0].Children[1], SubChildBusinessRule.BeforeValidationModels[1].Item1);
                    Assert.AreEqual(model.Children[1].Children[0], SubChildBusinessRule.BeforeValidationModels[2].Item1);
                    Assert.AreEqual(SubChildBusinessRule.Instances[0], SubChildBusinessRule.BeforeValidationModels[0].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[1], SubChildBusinessRule.BeforeValidationModels[1].Item2);
                    Assert.AreEqual(SubChildBusinessRule.Instances[2], SubChildBusinessRule.BeforeValidationModels[2].Item2);
                }
            }
        }

        [Test]
        public void GenericChildBusinessRuleShouldWork()
        {
            var child1 = new ConcreteGenericChildChild();
            var child2 = new ConcreteGenericChildChild2();
            var child3 = new ConcreteGenericChildChild {Name = "Test"};
            var child4 = new ConcreteGenericChildChild2();
            var model = new GenericChildParent
            {
                Children = new List<GenericChildChild>
                {
                    child1,
                    child2,
                    child3
                },
                Relation = child4
            };
            var validator = Container.GetInstance<IValidator<GenericChildParent>>();

            var valResult = validator.Validate(model);
            Assert.IsFalse(valResult.IsValid);
            Assert.AreEqual(3, valResult.Errors.Count);
            Assert.AreEqual(valResult.Errors[0].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[0].PropertyName, "Children[0]");
            Assert.AreEqual(valResult.Errors[0].AttemptedValue, child1);
            Assert.AreEqual(valResult.Errors[1].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[1].PropertyName, "Children[1]");
            Assert.AreEqual(valResult.Errors[1].AttemptedValue, child2);
            Assert.AreEqual(valResult.Errors[2].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[2].PropertyName, "Relation");
            Assert.AreEqual(valResult.Errors[2].AttemptedValue, child4);

            var childValidator = Container.GetInstance<IValidator<GenericChildChild>>();
            valResult = childValidator.Validate(child1);
            Assert.IsTrue(valResult.IsValid);
        }

        [Test]
        public void GenericRootChildBusinessRuleShouldWork()
        {
            var child1 = new ConcreteGenericRootChildChild();
            var child2 = new ConcreteGenericRootChildChild2();
            var child3 = new ConcreteGenericRootChildChild { Name = "Test" };
            var child4 = new ConcreteGenericRootChildChild2();
            var model = new GenericRootChildParent
            {
                Children = new List<GenericRootChildChild>
                {
                    child1,
                    child2,
                    child3
                },
                Relation = child4
            };
            var validator = Container.GetInstance<IValidator<GenericRootChildParent>>();

            var valResult = validator.Validate(model);
            Assert.IsFalse(valResult.IsValid);
            Assert.AreEqual(3, valResult.Errors.Count);
            Assert.AreEqual(valResult.Errors[0].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[0].PropertyName, "Children[0]");
            Assert.AreEqual(valResult.Errors[0].AttemptedValue, child1);
            Assert.AreEqual(valResult.Errors[1].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[1].PropertyName, "Children[1]");
            Assert.AreEqual(valResult.Errors[1].AttemptedValue, child2);
            Assert.AreEqual(valResult.Errors[2].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[2].PropertyName, "Relation");
            Assert.AreEqual(valResult.Errors[2].AttemptedValue, child4);

            var childValidator = Container.GetInstance<IValidator<GenericRootChildChild>>();
            valResult = childValidator.Validate(child1);
            Assert.IsTrue(valResult.IsValid);
        }

        [Test]
        public void GenericRootBusinessRuleShouldWork()
        {
            var child1 = new GenericRootModel();
            var child2 = new GenericRootModel();
            var child3 = new GenericRootModel { Name = "Test" };
            var child4 = new GenericRootModel();
            var model = new GenericRootModel
            {
                Children = new List<GenericRootModel>
                {
                    child1,
                    child2,
                    child3
                },
                Relation = child4
            };
            var validator = Container.GetInstance<IValidator<GenericRootModel>>();

            var valResult = validator.Validate(model);
            Assert.IsFalse(valResult.IsValid);
            Assert.AreEqual(4, valResult.Errors.Count);
            Assert.AreEqual(valResult.Errors[0].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[0].PropertyName, "Children[0]");
            Assert.AreEqual(valResult.Errors[0].AttemptedValue, child1);
            Assert.AreEqual(valResult.Errors[1].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[1].PropertyName, "Children[1]");
            Assert.AreEqual(valResult.Errors[1].AttemptedValue, child2);
            Assert.AreEqual(valResult.Errors[2].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[2].PropertyName, "Relation");
            Assert.AreEqual(valResult.Errors[2].AttemptedValue, child4);
            Assert.AreEqual(valResult.Errors[3].ErrorMessage, "Name should not be empty");
            Assert.AreEqual(valResult.Errors[3].PropertyName, "");
            Assert.AreEqual(valResult.Errors[3].AttemptedValue, model);

            var childValidator = Container.GetInstance<IValidator<GenericRootModel>>();
            valResult = childValidator.Validate(child1);
            Assert.IsFalse(valResult.IsValid);
        }
    }
}
