using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation;
using FluentValidation;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Util;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Parameters;
using Ninject.Syntax;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain.Extensions;
using PowerArhitecture.Domain.Specifications;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.DataAccess.NHEventListeners
{
    [NhEventListener(Order = int.MaxValue)] //Validation must be executed as last
    [NhEventListenerType(typeof(IFlushEventListener), Order = int.MinValue)]
    [NhEventListenerType(typeof(IAutoFlushEventListener), Order = int.MinValue)]
    [NhEventListenerType(typeof(ISaveOrUpdateEventListener), Order = int.MinValue)]
    [NhEventListenerType(typeof(IDeleteEventListener), Order = int.MinValue)]
    public class ValidatePreInsertUpdateDeleteEventHandler : BaseEventsHandler
        <
            //Reset cache
            SessionFlushingEvent,
            EntityDeletingEvent,
            EntitySavingOrUpdatingEvent,
            //Validation events
            EntitySavingEvent
        >,
        //Validation events
        IPreCollectionUpdateEventListener,
        IPreUpdateEventListener,
        IPreDeleteEventListener
    {
        private readonly ILogger _logger;
        private readonly IResolutionRoot _resolutionRoot;
        private readonly HashSet<IEntity> _validatedEntities = new HashSet<IEntity>(); 

        public ValidatePreInsertUpdateDeleteEventHandler(ILogger logger, IResolutionRoot resolutionRoot)
        {
            _logger = logger;
            _resolutionRoot = resolutionRoot;
        }

        private class ValidationInfo
        {
            public ValidationInfo(ISession childSession, IValidator validator, IEntity model, Type modelType, 
                bool root, string[] ruleSets, object contextDataFiller)
            {
                ChildSession = childSession;
                Validator = validator;
                RuleSets = ruleSets;
                Model = model;
                ModelType = modelType;
                Root = root;
                ContextDataFiller = contextDataFiller;
            }

            public ISession ChildSession { get; }

            public IValidator Validator { get; }

            public IEntity Model { get; }

            public Type ModelType { get; }

            public bool Root { get; }

            public string[] RuleSets { get; }

            public object ContextDataFiller { get; }

            public ValidationInfo Next { get; set; }

            public void Append(ValidationInfo info)
            {
                var curr = this;
                while (curr.Next != null)
                {
                    curr = curr.Next;
                }
                curr.Next = info;
            }
        }

        private async Task ValidateAsync(object entity, ISession session, EntityMode mode, string[] ruleSet)
        {
            var valInfo = GetValidationInfo(entity, session, mode, ruleSet);
            if (valInfo == null)
            {
                return;
            }

            var validationResult = await ValidatorExtensions.ValidateAsync(valInfo.Validator, valInfo.Model,
                valInfo.RuleSets, valInfo.ContextDataFiller);
            if (!validationResult.IsValid)
            {
                _validatedEntities.Clear();
                throw new ExtendedValidationException(validationResult.Errors, entity, valInfo.ModelType, valInfo.RuleSets);
            }
            _validatedEntities.Add(valInfo.Model);
        }

        private void Validate(object entity, ISession session, EntityMode mode, string[] ruleSet)
        {
            var valInfo = GetValidationInfo(entity, session, mode, ruleSet);
            if (valInfo == null)
            {
                return;
            }

            do
            {
                var validationResult = ValidatorExtensions.Validate(valInfo.Validator, valInfo.Model,
                    valInfo.RuleSets, valInfo.ContextDataFiller, new Dictionary<string, object>
                    {
                        { ValidationContextExtensions.RootAutoValidationKey, valInfo.Root },
                        { ValidationContextExtensions.AutoValidationKey, true }
                    });
                if (!validationResult.IsValid)
                {
                    _validatedEntities.Clear();
                    throw new ExtendedValidationException(validationResult.Errors, entity, valInfo.ModelType,
                        valInfo.RuleSets);
                }
                _validatedEntities.Add(valInfo.Model);
                valInfo = valInfo.Next;
            } while (valInfo != null);
        }

        private ValidationInfo GetValidationInfo(object item, ISession session, EntityMode mode, string[] ruleSets)
        {
            if (item == null || mode != EntityMode.Poco)
            {
                return null;
            }
            var entity = item as IEntity;
            if (entity == null)
            {
                // Will happen when an entity is mapped as a dictionary
                _logger.Debug("Skip validation for type '{0}' as is not castable to IEntity", item.GetType());
                return null;
            }
            var validableEntity = GetValidableRootEntity(item);
            if (validableEntity == null)
            {
                // When a child is removed from the parent by removing the relation on both sides the Aggregate root will be always null
                if (item is IAggregateChild && ruleSets.Contains(ValidationRuleSet.Delete))
                {
                    return GetValidationInfo(entity, session, new[] { ValidationRuleSet.Delete }, false);
                }
                return null;
            }
            var rootEntity = validableEntity as IEntity;
            if (rootEntity == null)
            {
                throw new PowerArhitectureException(
                    $"Invalid usage of IAutoValidated interface on the type {validableEntity.GetType()}. " +
                    "IAutoValidated can be only applied on types that implements IEntity.");
            }

            ValidationInfo validationInfo = null;
            // If the changing entity is a child of the validable entity, we need to find out the correct ruleset for the validable entity
            if (item != validableEntity)
            {
                var persistenceContext = session.GetSessionImplementation().PersistenceContext;
                var entry = persistenceContext.GetEntry(validableEntity);
                switch (entry.Status)
                {
                    // If the parent will be deleted then a child cannot be inserted or updated
                    case Status.Deleted:
                        ruleSets = new[] {ValidationRuleSet.Delete};
                        break;
                    // If a child entity will be deleted or inserted we need to validate it separately
                    case Status.Loaded:
                        if (!entry.ExistsInDatabase)
                        {
                            ruleSets = ValidationRuleSet.AttributeInsert;
                            break;
                        }
                        if (_validatedEntities.Contains(entity))
                        {
                            break;
                        }
                        // Validate the child entity only if the root is already inserted and we are appending a new child or
                        // we are removing one
                        if (ruleSets.Contains(ValidationRuleSet.Insert))
                        {
                            validationInfo = GetValidationInfo(entity, session, new[] {ValidationRuleSet.Insert}, false);
                        }
                        else if (ruleSets.Contains(ValidationRuleSet.Delete))
                        {
                            validationInfo = GetValidationInfo(entity, session, new[] { ValidationRuleSet.Delete }, false);
                        }
                        ruleSets = ValidationRuleSet.AttributeUpdate;
                        break;
                    // If the parent will be saved then a child cannot be deleted but can be updated (switched from one parent to another)
                    case Status.Saving:
                        ruleSets = ValidationRuleSet.AttributeInsert;
                        if (ruleSets.Contains(ValidationRuleSet.Update) && !_validatedEntities.Contains(entity))
                        {
                            validationInfo = GetValidationInfo(entity, session, new[] { ValidationRuleSet.Update }, false);
                        }
                        break;
                    case Status.ReadOnly:
                        throw new PowerArhitectureException(
                            $"Changing a child entity of a readonly auto validable parent is not permitted (Child: {item}, Parent: {validableEntity}).");
                    default:
                        throw new PowerArhitectureException(
                            $"Unsupported entity entry status {entry.Status} while auto validating the entity {validableEntity}");
                }
            }

            if (_validatedEntities.Contains(rootEntity))
            {
                _logger.Debug("Entity of type '{0}' was already validated", item.GetType());
                return validationInfo;
            }

            if (
                    (ruleSets.Contains(ValidationRuleSet.Update) && !validableEntity.ValidateOnUpdate) ||
                    (ruleSets.Contains(ValidationRuleSet.Delete) && !validableEntity.ValidateOnDelete) ||
                    (ruleSets.Contains(ValidationRuleSet.Insert) && !validableEntity.ValidateOnInsert)
                )
            {
                return null;
            }

            return GetValidationInfo(rootEntity, session, ruleSets, true, validationInfo);
        }

        private ValidationInfo GetValidationInfo(IEntity entity, ISession session, string[] ruleSets, bool root, ValidationInfo valInfo = null)
        {
            var type = entity.GetTypeUnproxied();
            // Validators are singleton
            var validator = (IValidator)_resolutionRoot.Get(typeof(IValidator<>).MakeGenericType(type));
            var validatorEx = validator as IValidatorExtended;
            var sessionContext = session.GetSessionImplementation().UserData as SessionContext;
            object contextFiller = null;

            if (sessionContext == null && validatorEx != null && !validatorEx.CanValidateWithoutContextFiller && validatorEx.HasValidationContextFiller)
            {
                throw new PowerArhitectureException(
                    $"Entity of type '{type}' cannot be auto validated as ISession is not managed and a IValidationContextFiller<> is required. " +
                    "Hint: use a managed ISession or remove IAutoValidate from the type or set CanValidateWithoutContextFiller on the type validator to false");
            }

            // For validation we want to have a clean session (cache lvl 1) that share the same connection and transaction from the current one
            // so that autoflush mode will not flush before querying in the validator (i.e. check if user exists).
            // FlushMode is set to Never to ensure that validation process will not trigger any update/insert statements
            var childSession = valInfo?.ChildSession;
            if (childSession == null)
            {
                childSession = session.GetSession(EntityMode.Poco);
                childSession.FlushMode = FlushMode.Never;
            }

            if (sessionContext != null)
            {
                contextFiller = sessionContext.ResolutionRoot.TryGet(
                    typeof(IValidationContextFiller<>).MakeGenericType(type),
                    new TypeMatchingConstructorArgument(typeof(ISession), (context, target) => childSession, true));
            }

            var newValInfo = new ValidationInfo(childSession, validator, entity, type, root, ruleSets, contextFiller);
            if (valInfo == null)
            {
                return newValInfo;
            }
            valInfo.Append(newValInfo);
            return valInfo;
        }

        private IAutoValidated GetValidableRootEntity(object entity)
        {
            var currentChild = entity as IAggregateChild;
            while (currentChild != null)
            {
                var root = currentChild.AggregateRoot;
                return root == null ? null : GetValidableRootEntity(root);
            }

            var validableEntity = entity as IAutoValidated;
            return validableEntity;
        }

        // Validation for inserting has to be before the Id is set by NHibernate
        public override void Handle(EntitySavingEvent e)
        {
            var @event = e.Event;
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeInsert);
        }

        public override Task HandleAsync(EntitySavingEvent e, CancellationToken cancellationToken)
        {
            var @event = e.Event;
            return ValidateAsync(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeInsert);
        }

        public async Task OnPreUpdateCollectionAsync(PreCollectionUpdateEvent @event)
        {
            var owner = @event.AffectedOwnerOrNull;
            if (!ReferenceEquals(null, owner))
            {
                await ValidateAsync(owner, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeUpdate);
            }
        }

        public void OnPreUpdateCollection(PreCollectionUpdateEvent @event)
        {
            var owner = @event.AffectedOwnerOrNull;
            if (!ReferenceEquals(null, owner))
            {
                Validate(owner, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeUpdate);
            }
        }

        public async Task<bool> OnPreUpdateAsync(PreUpdateEvent @event)
        {
            await ValidateAsync(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeUpdate);
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeUpdate);
            return false;
        }

        public async Task<bool> OnPreDeleteAsync(PreDeleteEvent @event)
        {
            await ValidateAsync(@event.Entity, @event.Session, @event.Session.EntityMode, new [] {ValidationRuleSet.Delete});
            return false;
        }

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, new[] { ValidationRuleSet.Delete });
            return false;
        }


        public override void Handle(SessionFlushingEvent message)
        {
            _validatedEntities.Clear();
        }

        public override Task HandleAsync(SessionFlushingEvent message, CancellationToken cancellationToken)
        {
            _validatedEntities.Clear();
            return Task.CompletedTask;
        }

        public override void Handle(EntityDeletingEvent message)
        {
            _validatedEntities.Clear();
        }

        public override Task HandleAsync(EntityDeletingEvent message, CancellationToken cancellationToken)
        {
            _validatedEntities.Clear();
            return TaskHelper.CompletedTask;
        }

        public override void Handle(EntitySavingOrUpdatingEvent message)
        {
            _validatedEntities.Clear();
        }

        public override Task HandleAsync(EntitySavingOrUpdatingEvent message, CancellationToken cancellationToken)
        {
            _validatedEntities.Clear();
            return TaskHelper.CompletedTask;
        }

    }
}
