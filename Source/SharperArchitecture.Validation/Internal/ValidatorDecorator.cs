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
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Validation.Events;
using SharperArchitecture.Validation.Specifications;
using SimpleInjector;

namespace SharperArchitecture.Validation.Internal
{
    internal class ValidatorDecorator<TModel> : IValidator<TModel>, IEnumerable<IValidationRule>
    {
        private const string DataKey = "_businessRules";
        private readonly AbstractValidator<TModel> _validator;
        private readonly Container _container;
        private readonly ValidatorCache _cache;

        public ValidatorDecorator(IValidator<TModel> validator, Container container, IEventPublisher eventPublisher, ValidatorCache cache)
        {
            _validator = (AbstractValidator<TModel>)validator;
            _container = container;
            _cache = cache;
            var registrations = container.GetCurrentRegistrations()
                .Where(o => o.ServiceType.IsInterface &&
                    o.ServiceType.IsGenericType && 
                    o.ServiceType.GetGenericTypeDefinition() == typeof(IBusinessRule<,>))
                .Select(o => new {Producer = o, Type = o.ServiceType.GetGenericType(typeof(IBusinessRule<,>))})
                .Where(o => o.Type != null)
                .ToList();
            _cache.AddRootProducers<TModel>(registrations
                .Where(o => o.Type.GetGenericArguments()[0] == typeof(TModel))
                .Select(o => o.Producer));
            _cache.AddChildProducers<TModel>(registrations
                .Where(o => o.Type.GetGenericArguments()[1] == typeof(TModel))
                .Select(o => o.Producer));
            AddProducersForGenericRules();

            var absValidator = (AbstractValidator<TModel>)validator;
            var addRuleMethod = absValidator.GetType().GetMethod("AddRule", BindingFlags.Instance | BindingFlags.NonPublic);
            addRuleMethod.Invoke(absValidator, new object[] { new BusinessRulesValidator(GetRulesToValidate) });
            //absValidator.AddRule(new BusinessRulesValidator(GetRulesToValidate));
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
            foreach (var producer in _cache.GetChildProducers<TModel>().Where(o => dict.ContainsKey(o)))
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

        private void AddProducersForGenericRules()
        {
            var childModels = new HashSet<Type>();
            var valRules = new List<IValidationRule>(_validator);
            // Find all child models that are realted to this one by traversing all validation rules
            while (valRules.Count > 0)
            {
                var valRule = valRules.First();
                valRules.Remove(valRule);
                foreach (var propVal in valRule.Validators)
                {
                    Type childValType = null;
                    if (propVal is IChildValidatorAdaptor childRule)
                    {
                        childValType = childRule.ValidatorType.GetGenericType(typeof(IValidator<>));
                    }

                    if (childValType == null)
                    {
                        continue;
                    }

                    var childModel = childValType.GetGenericArguments()[0];
                    if (childModel == typeof(TModel))
                    {
                        continue;
                    }

                    childModels.Add(childModel);
                    valRules.AddRange(_container.GetInstance(childValType) as IEnumerable<IValidationRule> ??
                                      new List<IValidationRule>());
                }
            }

            // Add root and child producers for the valid root/child combinations
            foreach (var genRule in _cache.GetGenericBusinessRules())
            {
                var bussinesRuleType = genRule.GetGenericType(typeof(IBusinessRule<,>));
                var rootType = bussinesRuleType.GetGenericArguments()[0];
                var childType = bussinesRuleType.GetGenericArguments()[1];

                // Check if the current model can be applied as TRoot
                if (rootType == typeof(TModel) ||
                    (rootType.IsGenericParameter && rootType.GetGenericParameterConstraints().All(o => o.IsAssignableFrom(typeof(TModel)))))
                {
                    if (rootType == childType)
                    {
                        var genArgs = genRule.GetGenericArguments().Select(args => typeof(TModel)).ToArray();
                        var producer = _container.GetRegistration(genRule.MakeGenericType(genArgs.ToArray()));
                        _cache.AddRootProducers<TModel>(producer);
                        _cache.AddChildProducers(typeof(TModel), producer);
                    }

                    foreach (var childModel in childModels)
                    {
                        // Check if the current childmodel can be applied as TChild
                        if (childType == childModel ||
                            (childType.IsGenericParameter &&
                             childType.GetGenericParameterConstraints().All(o => o.IsAssignableFrom(childModel))))
                        {
                            var genArgs = new List<Type>();
                            if (rootType.IsGenericParameter)
                            {
                                genArgs.Add(typeof(TModel));
                            }
                            if (childType.IsGenericParameter)
                            {
                                genArgs.Add(childModel);
                            }
                            // Get registration by implementation type in order to avoid duplicates 
                            var producer = _container.GetRegistration(genRule.MakeGenericType(genArgs.ToArray()));
                            _cache.AddRootProducers<TModel>(producer);
                            _cache.AddChildProducers(childModel, producer);
                        }
                    }
                }
            }
        }

        private Dictionary<InstanceProducer, IBusinessRule> ExecuteBeforeValidation(object instance, ValidationContext context)
        {
            var list = new Dictionary<InstanceProducer, IBusinessRule>();
            Action<IBusinessRule, InstanceProducer> action = (businessRule, producer) =>
            {
                var validRuleSets = businessRule.RuleSets ?? new string[] { };
                if (!validRuleSets.Any() && !context.Selector.CanExecute(RuleSetValidationRule.GetRule(null), "", context))
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
            foreach (var producer in _cache.GetRootProducers<TModel>())
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
