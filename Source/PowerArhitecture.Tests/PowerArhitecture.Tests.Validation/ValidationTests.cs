using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Tests.Common;
using PowerArhitecture.Tests.Validation.Models;
using PowerArhitecture.Validation;

namespace PowerArhitecture.Tests.Validation
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
        public void ValidateNesetdModelWithBusinessRule()
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
        public async Task ValidateNesetdModelWithBusinessRuleAsync()
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
    }
}
