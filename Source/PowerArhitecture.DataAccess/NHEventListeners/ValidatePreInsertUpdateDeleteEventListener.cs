using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation;
using FluentValidation;
using FluentValidation.Internal;
using NHibernate;
using NHibernate.Event;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Parameters;
using PowerArhitecture.Domain.Specifications;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.DataAccess.NHEventListeners
{
    [NhEventListener(Order = int.MaxValue)] //Validation must be executed as last
    [NhEventListenerType(typeof(IFlushEventListener), Order = int.MinValue)]
    [NhEventListenerType(typeof(IAutoFlushEventListener), Order = int.MinValue)]
    [NhEventListenerType(typeof(ISaveOrUpdateEventListener), Order = int.MinValue)]
    [NhEventListenerType(typeof(IDeleteEventListener), Order = int.MinValue)]
    public class ValidatePreInsertUpdateDeleteEventListener :
        //Reset cache
        IFlushEventListener,
        IAutoFlushEventListener,
        ISaveOrUpdateEventListener,
        IDeleteEventListener,

        //Validation events
        IListener<EntitySavingEvent>,
        IPreCollectionUpdateEventListener,
        IPreUpdateEventListener,
        IPreDeleteEventListener
    {
        private readonly ILogger _logger;
        private readonly Lazy<ISessionManager> _lazySessionManager;
        private readonly HashSet<IAutoValidated> _validatedEntities = new HashSet<IAutoValidated>(); 

        public ValidatePreInsertUpdateDeleteEventListener(ILogger logger, Lazy<ISessionManager> sessionManager)
        {
            _logger = logger;
            _lazySessionManager = sessionManager;
        }

        protected ISessionManager SessionManager { get { return _lazySessionManager.Value; } }

        private void Validate(object entity, ISession session, EntityMode mode, string ruleSet)
        {
            Validate(entity, session, mode, new [] { ruleSet });
        }

        private void Validate(object item, ISession session, EntityMode mode, string[] ruleSets)
        {
            if (item == null || mode != EntityMode.Poco)
                return;
            //Example 1: UnitOfWork inside a HttpRequest that have a Session
            //Example 2: UnitOfWork outside HttpRequest (SessionProvider will return always a new session) 
            var sessionInfo = _lazySessionManager.Value.GetSessionInfo(session);
            if (sessionInfo == null)
            {
                _logger.Warn("Skip validation for type '{0}' as session is not managed", item.GetType());
                return; //Unmanaged session
            }
            var entity = item as IEntity;
            if (entity == null)
            {
                _logger.Warn("Skip validation for type '{0}' as is not castable to IEntity", item.GetType());
                return;
            }
            var validableEntity = GetValidableEntity(item);
            if (validableEntity == null)
            {
                return;
            }

            if (
                    (ruleSets.Contains(ValidationRuleSet.Update) && !validableEntity.ValidateOnUpdate) ||
                    (ruleSets.Contains(ValidationRuleSet.Delete) && !validableEntity.ValidateOnDelete) ||
                    (ruleSets.Contains(ValidationRuleSet.Insert) && !validableEntity.ValidateOnInsert)
                )
            {
                return;
            }

            if (_validatedEntities.Contains(validableEntity))
            {
                _logger.Debug("Entity for type '{0}' was already validated", item.GetType());
                return;
            }

            //For validation we want to have a clean session (cache lvl 1) that share the same connection and transaction from the current one
            //so that autoflush mode will not flush before querying in the validator (i.e. check if user exists)
            //FlushMode is set to Never to ensure that validation process will not trigger any update/insert statements
            var childSession = session.GetSession(EntityMode.Poco);
            var props = sessionInfo.SessionProperties;
            childSession.FlushMode = FlushMode.Never;
            if (props.SessionResolutionRoot == null) //When session is manually created, skip validation
            {
                _logger.Warn("Skip validation for type '{0}' as session was manually created", item.GetType());
                return;
            }
            var type = entity.GetTypeUnproxied();
            var validator = (IValidator)props.SessionResolutionRoot.Get(typeof(IValidator<>).MakeGenericType(type),
                new TypeMatchingConstructorArgument(typeof(ISession), (context, target) => childSession, true));

            var validationResult = validator.Validate(GetValidationContext(type, entity, ruleSets));
            if (!validationResult.IsValid)
            {
                _validatedEntities.Clear();
                throw new PAValidationException(validationResult.Errors, entity, type, ruleSets);
            }
            _validatedEntities.Add(validableEntity);
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

        private ValidationContext GetValidationContext(Type type, object entity, IEnumerable<string> ruleSets)
        {
            return typeof (ValidationContext<>).MakeGenericType(type).GetConstructors()
                .First(c => c.GetParameters().Length == 3)
                .Invoke(new[] {entity, new PropertyChain(), new PARulesetValidatorSelector(ruleSets)}) as ValidationContext;
        }

        //Validation for inserting has to be before the Id is set by NHibernate
        public void Handle(EntitySavingEvent message)
        {
            var @event = message.Message;
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeInsert);
        }

        public void OnPreUpdateCollection(PreCollectionUpdateEvent @event)
        {
            var owner = @event.AffectedOwnerOrNull;
            if (!ReferenceEquals(null, owner))
            {
                Validate(owner, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeUpdate);
            }
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeUpdate);
            return false;
        }

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.Delete);
            return false;
        }

        public async Task OnFlush(FlushEvent @event, bool async)
        {
            _validatedEntities.Clear();
            await Task.Yield();
        }

        public async Task OnAutoFlush(AutoFlushEvent @event, bool async)
        {
            _validatedEntities.Clear();
            await Task.Yield();
        }

        public async Task OnSaveOrUpdate(SaveOrUpdateEvent @event, bool async)
        {
            _validatedEntities.Clear();
            await Task.Yield();
        }

        public void OnDelete(DeleteEvent @event)
        {
            _validatedEntities.Clear();
        }

        public void OnDelete(DeleteEvent @event, ISet<object> transientEntities)
        {
            _validatedEntities.Clear();
        }
    }
}
