using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Validation.Events;
using PowerArhitecture.Validation.Specifications;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace PowerArhitecture.Validation.Internal
{
    internal class ValidatorDecorator<TModel> : IValidator<TModel>, IEnumerable<IValidationRule>
    {
        private readonly AbstractValidator<TModel> _validator;

        private const string DataKey = "_businessRules";

        private static List<InstanceProducer> _childProducers;
        private static List<InstanceProducer> _rootProducers;

        public ValidatorDecorator(IValidator<TModel> validator, Container container, IEventPublisher eventPublisher)
        {
            _validator = (AbstractValidator<TModel>)validator;
            // Cache business rules that have different generic argument type for TRoot and TChild
            var registrations = container.GetCurrentRegistrations()
                .Where(o => o.ServiceType.IsInterface &&
                    o.ServiceType.IsGenericType && 
                    o.ServiceType.GetGenericTypeDefinition() == typeof(IBusinessRule<,>))
                .Select(o => new {Producer = o, Type = o.ServiceType.GetGenericType(typeof(IBusinessRule<,>))})
                .Where(o => o.Type != null && o.Type.GetGenericArguments().Any(a => a != typeof(TModel)))
                .ToList();
            _rootProducers = registrations
                .Where(o => o.Type.GetGenericArguments()[0] == typeof(TModel))
                .Select(o => o.Producer)
                .ToList();
            _childProducers = registrations
                .Where(o => o.Type.GetGenericArguments()[1] == typeof(TModel))
                .Select(o => o.Producer)
                .ToList();

            // Cache generic business rules that cannot be retrieved with container.GetCurrentRegistrations and also rules
            // that have the same generic argument type for TRoot and TChild
            var collProducer = container.GetRegistration(typeof(IEnumerable<IBusinessRule<TModel, TModel>>));
            if (collProducer != null)
            {
                _rootProducers.Add(collProducer);
                _childProducers.Add(collProducer);
            }

            var absValidator = (AbstractValidator<TModel>)validator;
            absValidator.AddRule(new BusinessRulesValidator(GetRulesToValidate));
            eventPublisher.Publish(new ValidatorCreatedEvent(validator, typeof(TModel)));
        }

        public ValidationResult Validate(object instance)
        {
            if (!CanValidateInstancesOfType(instance.GetType()))
            {
                throw new InvalidOperationException(
                    $"Cannot validate instances of type '{instance.GetType().Name as object}'. " +
                    $"This validator can only validate instances of type '{typeof(TModel).Name}'.");
            }
            return Validate((TModel)instance);
        }

        public ValidationResult Validate(TModel instance)
        {
            var context = new ValidationContext<TModel>(instance, new PropertyChain(),
                ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory());
            context.RootContextData[DataKey] = ExecuteBeforeValidation(instance, context);
            return _validator.Validate(context);
        }

        public ValidationResult Validate(ValidationContext context)
        {
            UpdateContext(context);
            return ((IValidator)_validator).Validate(context);
        }

        public async Task<ValidationResult> ValidateAsync(object instance, CancellationToken cancellation = new CancellationToken())
        {
            if (!CanValidateInstancesOfType(instance.GetType()))
            {
                throw new InvalidOperationException(
                    $"Cannot validate instances of type '{instance.GetType().Name as object}'. " +
                    $"This validator can only validate instances of type '{typeof(TModel).Name}'.");
            }
            return await ValidateAsync((TModel) instance, cancellation);
        }

        public Task<ValidationResult> ValidateAsync(TModel instance, CancellationToken cancellation = new CancellationToken())
        {
            var context = new ValidationContext<TModel>(instance, new PropertyChain(),
                ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory());
            context.RootContextData[DataKey] = ExecuteBeforeValidation(instance, context);
            return _validator.ValidateAsync(context, cancellation);
        }

        public Task<ValidationResult> ValidateAsync(ValidationContext context, CancellationToken cancellation = new CancellationToken())
        {
            UpdateContext(context);
            return ((IValidator)_validator).ValidateAsync(context, cancellation);
        }

        public IValidatorDescriptor CreateDescriptor()
        {
            return _validator.CreateDescriptor();
        }

        public bool CanValidateInstancesOfType(Type type)
        {
            return ((IValidator)_validator).CanValidateInstancesOfType(type);
        }

        public CascadeMode CascadeMode
        {
            get { return _validator.CascadeMode; }
            set { _validator.CascadeMode = value; }
        }

        private IEnumerable<IBusinessRule> GetRulesToValidate(ValidationContext context)
        {
            object dictObj;
            var dict = context.RootContextData.TryGetValue(DataKey, out dictObj)
                ? dictObj as Dictionary<InstanceProducer, IBusinessRule> ?? new Dictionary<InstanceProducer, IBusinessRule>()
                : new Dictionary<InstanceProducer, IBusinessRule>();
            foreach (var producer in _childProducers.Where(o => dict.ContainsKey(o)))
            {
                yield return dict[producer];
            }
        }

        private void UpdateContext(ValidationContext context)
        {
            object dictObj;
            if (context.RootContextData.TryGetValue(DataKey, out dictObj))
            {
                var dict = dictObj as Dictionary<InstanceProducer, IBusinessRule>;
                if (dict != null)
                {
                    var list = ExecuteBeforeValidation(context.InstanceToValidate, context);
                    foreach (var pair in list)
                    {
                        dict[pair.Key] = pair.Value;
                    }
                    return;
                }
            }
            context.RootContextData[DataKey] = ExecuteBeforeValidation(context.InstanceToValidate, context);
        }

        private Dictionary<InstanceProducer, IBusinessRule> ExecuteBeforeValidation(object instance, ValidationContext context)
        {
            var list = new Dictionary<InstanceProducer, IBusinessRule>();
            Action<IBusinessRule, InstanceProducer> action = (businessRule, producer) =>
            {
                var validRuleSets = businessRule.RuleSets ?? new string[] { };
                if (!validRuleSets.Any() && !context.Selector.CanExecute(RuleSetValidationRule.GetRule(""), "", context))
                {
                    return;
                }
                if (validRuleSets.Any() && !validRuleSets.Any(o => context.Selector.CanExecute(RuleSetValidationRule.GetRule(o), "", context)))
                {
                    return;
                }
                list.Add(producer, businessRule);
                businessRule.BeforeValidation(instance, context);
            };
            foreach (var producer in _rootProducers)
            {
                if (producer.ServiceType.IsGenericType &&
                    producer.ServiceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var businessRules = (IEnumerable<IBusinessRule>) producer.GetInstance();
                    foreach (var businessRule in businessRules)
                    {
                        action(businessRule, producer);
                    }
                }
                else
                {
                    action((IBusinessRule)producer.GetInstance(), producer);
                }
            }
            return list;
        }

        public IEnumerator<IValidationRule> GetEnumerator()
        {
            return _validator.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _validator).GetEnumerator();
        }
    }
}
