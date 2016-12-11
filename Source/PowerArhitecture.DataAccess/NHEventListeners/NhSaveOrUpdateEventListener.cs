using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Intercept;
using NHibernate.Persister.Entity;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.DataAccess.Extensions;

namespace PowerArhitecture.DataAccess.NHEventListeners
{
    [NhEventListener(ReplaceListener = typeof(DefaultSaveOrUpdateEventListener))]
    public class NhSaveOrUpdateEventListener : DefaultSaveOrUpdateEventListener
    {
        private readonly IAuditUserProvider _auditUserProvider;
        protected readonly IEventPublisher EventPublisher;

        public NhSaveOrUpdateEventListener(IAuditUserProvider auditUserProvider, IEventPublisher eventPublisher)
        {
            _auditUserProvider = auditUserProvider;
            EventPublisher = eventPublisher;
        }

        /// <summary>
        /// If a new versioned entity will be inserted we must set all audit properties here because of NH nullability check
        /// http://ayende.com/blog/3987/nhibernate-ipreupdateeventlistener-ipreinserteventlistener
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected override async Task<object> EntityIsTransientAsync(SaveOrUpdateEvent @event) //override this for fixing not-null transient property
        {
            //If entry is set then entity will be deleted otherwise will be inserted
            if (@event.Entry == null)
            {
                //We have to set only for transient entites here just for fixing not-null transient property exception from NH
                //for persistent entites we will update the audit properties in the preupdate listener (here we dont know if the entity is dirty)
                var dbSettings = Database.GetSessionFactoryInfo(@event.Session)?.DatabaseConfiguration;
                var reqLastModifiedProp = dbSettings?.Conventions?.RequiredLastModifiedProperty == true;
                var userType = SetAuditProperties(@event.Entity, reqLastModifiedProp);
                if (userType != null)
                {
                    SetCurrentUser(@event.Entity, await GetCurrentUserAsync(@event.Session, userType), reqLastModifiedProp);
                }
                await EventPublisher.PublishAsync(new EntitySavingAsyncEvent(@event));
            }
            return await base.EntityIsTransientAsync(@event);
        }

        protected override object EntityIsTransient(SaveOrUpdateEvent @event)
        {
            //If entry is set then entity will be deleted otherwise will be inserted
            if (@event.Entry == null)
            {
                //We have to set only for transient entites here just for fixing not-null transient property exception from NH
                //for persistent entites we will update the audit properties in the preupdate listener (here we dont know if the entity is dirty)
                var dbSettings = Database.GetSessionFactoryInfo(@event.Session)?.DatabaseConfiguration;
                var reqLastModifiedProp = dbSettings?.Conventions?.RequiredLastModifiedProperty == true;
                var userType = SetAuditProperties(@event.Entity, reqLastModifiedProp);
                if (userType != null)
                {
                    SetCurrentUser(@event.Entity, GetCurrentUser(@event.Session, userType), reqLastModifiedProp);
                }
                EventPublisher.Publish(new EntitySavingEvent(@event));
            }
            return base.EntityIsTransient(@event);
        }

        //This method can be called multiple times for a transient entity (because of cascades) so we will update the audit values only if they are not set
        private Type SetAuditProperties(object obj, bool requiredLastModifiedProp)
        {
            var entity = obj as IVersionedEntity;
            if (entity == null)
            {
                return null;
            }
            var currentDate = DateTime.UtcNow;
            if (entity.CreatedDate == DateTime.MinValue)
                obj.SetMemberValue("CreatedDate", currentDate);
            if (requiredLastModifiedProp && !entity.LastModifiedDate.HasValue)
                obj.SetMemberValue("LastModifiedDate", currentDate);

            var entityType = entity.GetTypeUnproxied();
            var genType = entityType.GetGenericType(typeof(IVersionedEntityWithUser<>));
            return genType?.GetGenericArguments()[0];
        }

        private object GetCurrentUser(ISession session, Type userType)
        {
            var currentUser = _auditUserProvider.GetCurrentUser(session, userType);
            if (currentUser == null)
            {
                throw new PowerArhitectureException("IAuditUserProvider failed to get the current user");
            }
            return currentUser;
        }

        private async Task<object> GetCurrentUserAsync(ISession session, Type userType)
        {
            var currentUser = await _auditUserProvider.GetCurrentUserAsync(session, userType);
            if (currentUser == null)
            {
                throw new PowerArhitectureException("IAuditUserProvider failed to get the current user");
            }
            return currentUser;
        }

        private void SetCurrentUser(object obj, object currentUser, bool requiredLastModifiedProp)
        {
            if (obj.GetMemberValue("CreatedBy") == null)
                obj.SetMemberValue("CreatedBy", currentUser);
            if (obj.GetMemberValue("LastModifiedBy") == null && requiredLastModifiedProp)
                obj.SetMemberValue("LastModifiedBy", currentUser);
        }
    }
}
