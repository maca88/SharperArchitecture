using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerArhitecture.Validation.Specifications;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace PowerArhitecture.Validation
{
    public class PAValidatorEngine : IValidatorEngine
    {
        private readonly IValidatorFactory _validatorFactory;
        private static readonly MethodInfo ValidateGenMultiRuleSetMethodInfo;
        static PAValidatorEngine()
        {
            ValidateGenMultiRuleSetMethodInfo = typeof(PAValidatorEngine).GetMethods()
                .Where(m => m.Name == "Validate")
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2
                            && x.Args.Length == 1
                            && x.Params[0].ParameterType == x.Args[0]
                            && x.Params[1].ParameterType == typeof(IEnumerable<string>))
                .Select(x => x.Method)
                .Single();
        }

        public PAValidatorEngine(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public ValidationResult Validate<T>(T model)
        {
            return _validatorFactory.GetValidator<T>().Validate(model);
        }

        public ValidationResult Validate<T>(T model, string ruleSet)
        {
            return _validatorFactory.GetValidator<T>().Validate(model, ruleSet: ruleSet);
        }

        public ValidationResult Validate(object model)
        {
            return _validatorFactory.GetValidator(model.GetType()).Validate(model);
        }

        public ValidationResult Validate(object model, IEnumerable<string> ruleSets)
        {
            return ValidateGenMultiRuleSetMethodInfo.MakeGenericMethod(model.GetType()).Invoke(this, new[] {model, ruleSets}) as ValidationResult;
        }

        public ValidationResult Validate<T>(T model, IEnumerable<string> ruleSets)
        {
            var ruleSetSelector = new PARulesetValidatorSelector(ruleSets);
            return _validatorFactory.GetValidator(model.GetType()).Validate(new ValidationContext<T>(model, new PropertyChain(), ruleSetSelector));
        }
    }
}
