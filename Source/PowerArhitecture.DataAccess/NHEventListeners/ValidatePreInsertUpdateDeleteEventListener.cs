using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation;
using PowerArhitecture.Validation.Specifications;
using FluentValidation;
using FluentValidation.Internal;
using NHibernate;
using NHibernate.Event;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Parameters;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess.NHEventListeners
{
    [NhEventListener(Order = int.MaxValue)] //Validation must be executed as last
    public class ValidatePreInsertUpdateDeleteEventListener : IPreCollectionUpdateEventListener, IPreInsertEventListener, IPreUpdateEventListener, IPreDeleteEventListener
    {
        private readonly ILogger _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly Lazy<ISessionManager> _lazySessionManager;

        public ValidatePreInsertUpdateDeleteEventListener(ILogger logger, IEventAggregator eventAggregator, Lazy<ISessionManager> sessionManager)
        {
            _logger = logger;
            _eventAggregator = eventAggregator;
            _lazySessionManager = sessionManager;
        }

        protected ISessionManager SessionManager { get { return _lazySessionManager.Value; } }

        public void OnPreUpdateCollection(PreCollectionUpdateEvent @event)
        {
            _eventAggregator.SendMessage(new EntityPreCollectionUpdatingEvent(@event));
            var owner = @event.AffectedOwnerOrNull;
            if (!ReferenceEquals(null, owner))
            {
                Validate(owner, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeUpdate);
            }
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            _eventAggregator.SendMessage(new EntityPreInsertingEvent(@event));
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeInsert);
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            _eventAggregator.SendMessage(new EntityPreUpdatingEvent(@event));
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.AttributeUpdate);
            return false;
        }

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            _eventAggregator.SendMessage(new EntityPreDeletingEvent(@event));
            Validate(@event.Entity, @event.Session, @event.Session.EntityMode, ValidationRuleSet.Delete);
            return false;
        }

        private void Validate(object entity, ISession session, EntityMode mode, string ruleSet)
        {
            Validate(entity, session, mode, new List<string> { ruleSet });
        }

        private void Validate(object item, ISession session, EntityMode mode, IEnumerable<string> ruleSets)
        {
            if (item == null || mode != EntityMode.Poco)
                return;
            //Example 1: UnitOfWork inside a HttpRequest that have a Session
            //Example 2: UnitOfWork outside HttpRequest (SessionProvider will return always a new session) 
            //We have to get the validation
            var sessionInfo = _lazySessionManager.Value.GetSessionInfo(session);
            if (sessionInfo == null)
            {
                _logger.Warn("Skip validation for type '{0}' as session is not managed", item.GetType());
                return; //Unmanaged session
            }
            var props = sessionInfo.SessionProperties;
            var entity = item as IEntity;
            if (entity == null)
            {
                _logger.Warn("Skip validation for type '{0}' as is not castable to IEntity", item.GetType());
                return;
            }
            var type = entity.GetTypeUnproxied();

            //For validation we want to have a clean session (cache lvl 1) that share the same connection and transaction from the current one
            //so that autoflush mode will not flush before querying in the validator (i.e. check if user exists)
            //FlushMode is set to Never to ensure that validation process will not trigger any update/insert statements
            var childSession = session.GetSession(EntityMode.Poco);
            childSession.FlushMode = FlushMode.Never;

            if (props.SessionResolutionRoot == null) //When session is manually created, skip validation
            {
                _logger.Warn("Skip validation for type '{0}' as session was manually created", item.GetType());
                return;
            }

            var validator = (IValidator)props.SessionResolutionRoot.Get(typeof(IValidator<>).MakeGenericType(type),
                new TypeMatchingConstructorArgument(typeof(ISession), (context, target) => childSession, true));

            var validationResult = validator.Validate(GetValidationContext(type, entity, ruleSets));
            if (!validationResult.IsValid)
            {
                throw new PAValidationException(validationResult.Errors, type, entity, ruleSets);
            }
                
        }

        private ValidationContext GetValidationContext(Type type, object entity, IEnumerable<string> ruleSets)
        {
            return typeof (ValidationContext<>).MakeGenericType(type).GetConstructors()
                .First(c => c.GetParameters().Length == 3)
                .Invoke(new[] {entity, new PropertyChain(), new PARulesetValidatorSelector(ruleSets)}) as ValidationContext;
        }

        
    }
}
