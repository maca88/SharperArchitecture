using System;
using System.Collections.Concurrent;
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
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.EventListeners;
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
            SessionFlushedEvent,
            EntityDeletingEvent,
            EntitySavingOrUpdatingEvent,
            TransactionCommittedEvent,
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
        private readonly ConcurrentDictionary<ISession, ConcurrentSet<IEntity>> _validatedEntities = new ConcurrentDictionary<ISession, ConcurrentSet<IEntity>>(); 

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
            session = session.Unwrap();
            var valInfo = GetValidationInfo(entity, session, mode, ruleSet);
            if (valInfo == null)
            {
                return;
            }

            do
            {
                _logger.Debug($"Auto validating entity Type:'{valInfo.ModelType}', " +
                              $"Id:'{valInfo.Model.GetId()}', RuleSets:'{string.Join(",", valInfo.RuleSets)}'");
                var validationResult = await ValidatorExtensions.ValidateAsync(valInfo.Validator, valInfo.Model,
                    valInfo.RuleSets, valInfo.ContextDataFiller, new Dictionary<string, object>
                    {
                        { ValidationContextExtensions.RootAutoValidationKey, valInfo.Root },
                        { ValidationContextExtensions.AutoValidationKey, true }
                    });
                if (!validationResult.IsValid)
                {
                    Clear(session);
                    throw new ExtendedValidationException(validationResult.Errors, entity, valInfo.ModelType,
                        valInfo.RuleSets);
                }
                var set = _validatedEntities.GetOrAdd(session, o => new ConcurrentSet<IEntity>());
                set.Add(valInfo.Model);
                valInfo = valInfo.Next;
            } while (valInfo != null);
        }

        private void Validate(object entity, ISession session, EntityMode mode, string[] ruleSet)
        {
            session = session.Unwrap();
            var valInfo = GetValidationInfo(entity, session, mode, ruleSet);
            if (valInfo == null)
            {
                return;
            }

            do
            {
                _logger.Debug($"Auto validating entity Type:'{valInfo.ModelType}', " +
                              $"Id:'{valInfo.Model.GetId()}', RuleSets:'{string.Join(",", valInfo.RuleSets)}'");
                var validationResult = ValidatorExtensions.Validate(valInfo.Validator, valInfo.Model,
                    valInfo.RuleSets, valInfo.ContextDataFiller, new Dictionary<string, object>
                    {
                        { ValidationContextExtensions.RootAutoValidationKey, valInfo.Root },
                        { ValidationContextExtensions.AutoValidationKey, true }
                    });
                if (!validationResult.IsValid)
                {
                    Clear(session);
                    throw new ExtendedValidationException(validationResult.Errors, entity, valInfo.ModelType,
                        valInfo.RuleSets);
                }
                var set = _validatedEntities.GetOrAdd(session, o => new ConcurrentSet<IEntity>());
                set.Add(valInfo.Model);
                valInfo = valInfo.Next;
            } while (valInfo != null);
        }

        private ValidationInfo GetValidationInfo(object item, ISession session, EntityMode mode, string[] ruleSets)
        {
            if (item == null || mode != EntityMode.Poco || !session.IsManaged())
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
            //var validableEntity = item as IAutoValidated;
            var rootValidableEntity = GetValidableRootEntity(item);
            if (rootValidableEntity == null)
            {
                if (!(item is IAggregateChild) || (_validatedEntities.ContainsKey(session) && _validatedEntities[session].Contains(entity)))
                {
                    return null;
                }
                var validableEntity = item as IAutoValidated;
                if (validableEntity == null)
                {
                    return null;
                }
                // When a child is removed from the parent by removing the relation on both sides the Aggregate root will be always null
                if (validableEntity.ValidateOnDelete && ruleSets.Contains(ValidationRuleSet.Delete))
                {
                    return GetValidationInfo(entity, session, new[] { ValidationRuleSet.Delete }, false);
                }
                // When a child is inserted without a root
                if (validableEntity.ValidateOnInsert && ruleSets.Contains(ValidationRuleSet.Insert))
                {
                    return GetValidationInfo(entity, session, new[] { ValidationRuleSet.Insert }, false);
                }
                // When a child without a root is being updated
                if (validableEntity.ValidateOnUpdate && ruleSets.Contains(ValidationRuleSet.Update))
                {
                    return GetValidationInfo(entity, session, new[] { ValidationRuleSet.Update }, false);
                }
                return null;
            }
            var rootEntity = rootValidableEntity as IEntity;
            if (rootEntity == null)
            {
                throw new PowerArhitectureException(
                    $"Invalid usage of IAutoValidated interface on the type {rootValidableEntity.GetType()}. " +
                    "IAutoValidated can be only applied on types that implements IEntity.");
            }

            ValidationInfo validationInfo = null;
            // If the changing entity is a child of the validable entity, we need to find out the correct ruleset for the validable entity
            if (item != rootValidableEntity)
            {
                var validableEntity = item as IAutoValidated;
                var persistenceContext = session.GetSessionImplementation().PersistenceContext;
                var entry = persistenceContext.GetEntry(rootValidableEntity);
                if (entry == null)
                {
                    // A child was linked to a parent that is not registered in the session context
                    return null;
                }
                switch (entry.Status)
                {
                    case Status.Deleted:
                        ruleSets = new[] {ValidationRuleSet.Delete};
                        break;
                    case Status.Loaded:
                        if (!entry.ExistsInDatabase)
                        {
                            ruleSets = ValidationRuleSet.AttributeInsert;
                            break;
                        }
                        if (_validatedEntities.ContainsKey(session) && _validatedEntities[session].Contains(entity))
                        {
                            break;
                        }
                        // Validate the child entity only if the root is already inserted and we are appending a new child or
                        // we are removing one
                        if (validableEntity?.ValidateOnInsert == true && ruleSets.Contains(ValidationRuleSet.Insert))
                        {
                            validationInfo = GetValidationInfo(entity, session, new[] {ValidationRuleSet.Insert}, false);
                        }
                        else if (validableEntity?.ValidateOnDelete == true && ruleSets.Contains(ValidationRuleSet.Delete))
                        {
                            validationInfo = GetValidationInfo(entity, session, new[] { ValidationRuleSet.Delete }, false);
                        }
                        else if (validableEntity?.ValidateOnUpdate == true && 
                                _validatedEntities.ContainsKey(session) && _validatedEntities[session].Contains(rootEntity) &&
                                ruleSets.Contains(ValidationRuleSet.Update))
                        {
                            // There is also a special case when attaching a persistent child to a transient root entity
                            // The root entity will be validated first
                            validationInfo = GetValidationInfo(entity, session, new[] { ValidationRuleSet.Update }, false);
                        }
                        ruleSets = ValidationRuleSet.AttributeUpdate;
                        break;
                    case Status.ReadOnly:
                        throw new PowerArhitectureException(
                            $"Changing a child entity of a readonly auto validable parent is not permitted (Child: {item}, Parent: {rootValidableEntity}).");
                    default:
                        throw new PowerArhitectureException(
                            $"Unsupported entity entry status {entry.Status} while auto validating the entity {rootValidableEntity}");
                }
            }

            if (_validatedEntities.ContainsKey(session) && _validatedEntities[session].Contains(rootEntity))
            {
                _logger.Debug("Entity of type '{0}' was already validated", item.GetType());
                return validationInfo;
            }

            if (
                    (ruleSets.Contains(ValidationRuleSet.Update) && !rootValidableEntity.ValidateOnUpdate) ||
                    (ruleSets.Contains(ValidationRuleSet.Delete) && !rootValidableEntity.ValidateOnDelete) ||
                    (ruleSets.Contains(ValidationRuleSet.Insert) && !rootValidableEntity.ValidateOnInsert)
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
                // Check whether the root is the same as the child in order to prevent a stackoverflow exception
                if (currentChild == root)
                {
                    return root as IAutoValidated;
                }
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

        public override void Handle(SessionFlushingEvent e)
        {
            Clear(e.Session);
        }

        public override Task HandleAsync(SessionFlushingEvent e, CancellationToken cancellationToken)
        {
            Clear(e.Session);
            return Task.CompletedTask;
        }

        public override void Handle(SessionFlushedEvent e)
        {
            Clear(e.Session);
        }

        public override Task HandleAsync(SessionFlushedEvent e, CancellationToken cancellationToken)
        {
            Clear(e.Session);
            return Task.CompletedTask;
        }

        public override void Handle(TransactionCommittedEvent e)
        {
            Clear(e.Session);
        }

        public override Task HandleAsync(TransactionCommittedEvent e, CancellationToken cancellationToken)
        {
            Clear(e.Session);
            return Task.CompletedTask;
        }

        public override void Handle(EntityDeletingEvent e)
        {
            Clear(e.Session);
        }

        public override Task HandleAsync(EntityDeletingEvent e, CancellationToken cancellationToken)
        {
            Clear(e.Session);
            return TaskHelper.CompletedTask;
        }

        public override void Handle(EntitySavingOrUpdatingEvent e)
        {
            Clear(e.Session);
        }

        public override Task HandleAsync(EntitySavingOrUpdatingEvent e, CancellationToken cancellationToken)
        {
            Clear(e.Session);
            return TaskHelper.CompletedTask;
        }

        private void Clear(ISession session)
        {
            if (!_validatedEntities.Any())
            {
                return;
            }
            session = session.Unwrap();
            ConcurrentSet<IEntity> set;
            if (_validatedEntities.TryRemove(session, out set))
            {
                set.Clear();
            }
        }
    }
}
