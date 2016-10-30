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
using NHibernate.Event;
using NHibernate.Util;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Parameters;
using Ninject.Syntax;
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
        private readonly HashSet<IAutoValidated> _validatedEntities = new HashSet<IAutoValidated>(); 

        public ValidatePreInsertUpdateDeleteEventHandler(ILogger logger, IResolutionRoot resolutionRoot)
        {
            _logger = logger;
            _resolutionRoot = resolutionRoot;
        }

        private class ValidationInfo
        {
            public ValidationInfo(IValidator validator, IAutoValidated model, Type modelType, string[] ruleSets, object contextDataFiller)
            {
                Validator = validator;
                RuleSets = ruleSets;
                Model = model;
                ModelType = modelType;
                ContextDataFiller = contextDataFiller;
            }

            public IValidator Validator { get; }

            public IAutoValidated Model { get; }

            public Type ModelType { get; }

            public string[] RuleSets { get; }

            public object ContextDataFiller { get; }
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
                throw new PAValidationException(validationResult.Errors, entity, valInfo.ModelType, valInfo.RuleSets);
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

            var validationResult = ValidatorExtensions.Validate(valInfo.Validator, valInfo.Model,
                valInfo.RuleSets, valInfo.ContextDataFiller);
            if (!validationResult.IsValid)
            {
                _validatedEntities.Clear();
                throw new PAValidationException(validationResult.Errors, entity, valInfo.ModelType, valInfo.RuleSets);
            }
            _validatedEntities.Add(valInfo.Model);
        }

        private ValidationInfo GetValidationInfo(object item, ISession session, EntityMode mode, string[] ruleSets)
        {
            if (item == null || mode != EntityMode.Poco)
                return null;
            //Example 1: UnitOfWork inside a HttpRequest that have a Session
            //Example 2: UnitOfWork outside HttpRequest (SessionProvider will return always a new session) 
            var entity = item as IEntity;
            if (entity == null)
            {
                _logger.Warn("Skip validation for type '{0}' as is not castable to IEntity", item.GetType());
                return null;
            }
            var validableEntity = GetValidableEntity(item);
            if (validableEntity == null)
            {
                return null;
            }

            if (
                    (ruleSets.Contains(ValidationRuleSet.Update) && !validableEntity.ValidateOnUpdate) ||
                    (ruleSets.Contains(ValidationRuleSet.Delete) && !validableEntity.ValidateOnDelete) ||
                    (ruleSets.Contains(ValidationRuleSet.Insert) && !validableEntity.ValidateOnInsert)
                )
            {
                return null;
            }

            if (_validatedEntities.Contains(validableEntity))
            {
                _logger.Debug("Entity of type '{0}' was already validated", item.GetType());
                return null;
            }

            entity = validableEntity as IEntity;
            var type = entity != null
                ? entity.GetTypeUnproxied()
                : validableEntity.GetType();
            // Validators are singleton
            var validator = (IValidator)_resolutionRoot.Get(typeof(IValidator<>).MakeGenericType(type));
            var validatorEx = validator as IValidatorExtended;
            var sessionContext = session.GetSessionImplementation().UserData as SessionContext;
            object contextFiller = null;

            if (sessionContext == null && validatorEx != null && !validatorEx.CanValidateWithoutContextFiller && validatorEx.HasValidationContextFiller)
            {
                _logger.Warn("Entity of type '{0}' won't be validated as ISession is not managed and a IValidationContextFiller<> is required", item.GetType());
                return null;
            }

            //For validation we want to have a clean session (cache lvl 1) that share the same connection and transaction from the current one
            //so that autoflush mode will not flush before querying in the validator (i.e. check if user exists)
            //FlushMode is set to Never to ensure that validation process will not trigger any update/insert statements
            var childSession = session.GetSession(EntityMode.Poco);
            childSession.FlushMode = FlushMode.Never;

            if (sessionContext != null)
            {
                contextFiller = sessionContext.ResolutionRoot.TryGet(
                    typeof(IValidationContextFiller<>).MakeGenericType(type),
                    new TypeMatchingConstructorArgument(typeof(ISession), (context, target) => childSession, true));
            }

            return new ValidationInfo(validator, validableEntity, type, ruleSets, contextFiller);
        }

        private IAutoValidated GetValidableEntity(object entity)
        {
            var validableEntity = entity as IAutoValidated;
            if (validableEntity != null)
            {
                return validableEntity;
            }

            var currentChild = entity as IAggregateChild;
            while (currentChild != null)
            {
                var root = currentChild.AggregateRoot;
                return root == null ? null : GetValidableEntity(root);
            }
            return null;
        }

        //Validation for inserting has to be before the Id is set by NHibernate
        public override void Handle(EntitySavingEvent message)
        {
            var @event = message.Message;
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeInsert);
        }

        public override Task HandleAsync(EntitySavingEvent message, CancellationToken cancellationToken)
        {
            var @event = message.Message;
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
