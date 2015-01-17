using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain.Specifications;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Intercept;
using NHibernate.Persister.Entity;

namespace PowerArhitecture.DataAccess.NHEventListeners
{
    [NhEventListener(ReplaceListener = typeof(DefaultSaveOrUpdateEventListener))]
    public class NhSaveOrUpdateEventListener : DefaultSaveOrUpdateEventListener
    {
        private readonly IUserCache _userCache;

        public NhSaveOrUpdateEventListener(IUserCache userCache)
        {
            _userCache = userCache;
        }

        /// <summary>
        /// If a new versioned entity will be inserted we must set all audit properties here because of NH nullability check
        /// http://ayende.com/blog/3987/nhibernate-ipreupdateeventlistener-ipreinserteventlistener
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected override object EntityIsTransient(SaveOrUpdateEvent @event) //override this for fixing not-null transient property
        {
            //If entry is set then entity will be deleted otherwise will be inserted
            if (@event.Entry == null)
            {
                SetAuditProperties(@event.Entity, @event.Session);
            }
            return base.EntityIsTransient(@event);
        }

        private void SetAuditProperties(object entity, ISession session)
        {
            
            var verEntity = entity as IVersionedEntity<IUser>;
            if (verEntity == null)
                return;
            var user = _userCache.GetCurrentUser();
            var currentUser = user == null ? null : session.Load<IUser>(user.Id);
            var currentDate = DateTime.UtcNow;
            var isNew = verEntity.IsTransient();
            
            if (isNew)
            {
                verEntity.SetMemberValue(o => o.CreatedDate, currentDate);
                verEntity.SetMemberValue(o => o.CreatedBy, currentUser);   
            }

            verEntity.SetMemberValue(o => o.LastModifiedDate, DateTime.UtcNow);
            verEntity.SetMemberValue(o => o.LastModifiedBy, currentUser);
        }

        /*
        protected override object EntityIsPersistent(SaveOrUpdateEvent @event)
        {
            //if (HasDirtyProperties(@event))
            //SetAuditProperties(@event.Entity, @event.Session);
            return base.EntityIsPersistent(@event);
        }
        
        protected override void EntityIsDetached(SaveOrUpdateEvent @event)
        {
            //if (HasDirtyProperties(@event))
            SetAuditProperties(@event.Entity, @event.Session);
            base.EntityIsDetached(@event);
        }
         
        private static bool HasDirtyProperties(SaveOrUpdateEvent @event)
        {
            ISessionImplementor session = @event.Session;
            var entry = @event.Entry;
            var entity = @event.Entity;
            if (entry == null || !entry.RequiresDirtyCheck(entity) || !entry.ExistsInDatabase || entry.LoadedState == null)
            {
                return false;
            }
            var persister = entry.Persister;

            var currentState = persister.GetPropertyValues(entity, session.EntityMode);

            var loadedState = entry.LoadedState;

            return persister.EntityMetamodel.Properties
                            .Where(
                                (property, i) =>
                                !LazyPropertyInitializer.UnfetchedProperty.Equals(currentState[i]) &&
                                property.Type.IsDirty(loadedState[i], currentState[i], session))
                            .Any();
        }
         
         */


    }
}
