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
        protected IEventAggregator EventAggregator { get; private set; }
        private readonly IUserCache _userCache;
        private readonly ISessionEventListener _sessionEventListener;

        private readonly ConcurrentDictionary<ISession, ConcurrentSet<object>> _cachedSessionEntites;

        public NhSaveOrUpdateEventListener(IEventAggregator eventAggregator, IUserCache userCache, ISessionEventListener sessionEventListener)
        {
            EventAggregator = eventAggregator;
            _userCache = userCache;
            _cachedSessionEntites = new ConcurrentDictionary<ISession, ConcurrentSet<object>>();
            _sessionEventListener = sessionEventListener;
        }
        /*
        protected override object EntityIsPersistent(SaveOrUpdateEvent @event)
        {
            CacheSession(@event.Session);
            if (!_cachedSessionEntites[@event.Session].Contains(@event.Entity) && HasDirtyProperties(@event))
                SetAuditProperties(@event.Entity, false, @event.Session);
            return base.EntityIsPersistent(@event);
        }
        */

        /// <summary>
        /// If a new versioned entity will be inserted we must set all audit properties here because of NH nullability check
        /// http://ayende.com/blog/3987/nhibernate-ipreupdateeventlistener-ipreinserteventlistener
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected override object EntityIsTransient(SaveOrUpdateEvent @event)
        {
            CacheSession(@event.Session);
            //If entry is set then entity will be deleted otherwise will be inserted
            if (@event.Entry == null)
            {
                SetAuditProperties(@event.Entity, true, @event.Session);
                //EventAggregator.SendMessage(new EntityInserting(@event));
            }
            return base.EntityIsTransient(@event);
        }

        private void SetAuditProperties(object entity, bool isNew, ISession session)
        {
            var verEntity = entity as IVersionedEntity<IUser>;
            if (verEntity == null)
                return;
            var user = _userCache.GetCurrentUser();
            var currentUser = user == null ? null : session.Load<IUser>(user.Id);
            var currentDate = DateTime.UtcNow;
            
            if (isNew)
            {
                verEntity.SetMemberValue(o => o.CreatedDate, currentDate);
                verEntity.SetMemberValue(o => o.CreatedBy, currentUser);   
            }

            verEntity.SetMemberValue(o => o.LastModifiedDate, DateTime.UtcNow);
            verEntity.SetMemberValue(o => o.LastModifiedBy, currentUser);

            _cachedSessionEntites[session].Add(entity);
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

        #region Session entites caching

        private void AfterTransactionCommit(ISession session)
        {
            ConcurrentSet<object> items;
            if (_cachedSessionEntites.TryRemove(session, out items))
                items.Clear();
            else
                throw new Exception("Unable to remove session from cached dictionary");
        }

        private void CacheSession(ISession session)
        {
            if (_cachedSessionEntites.ContainsKey(session)) return;
            _cachedSessionEntites.TryAdd(session, new ConcurrentSet<object>());
            _sessionEventListener.AddAListener(SessionListenerType.AfterCommit, session, AfterTransactionCommit);
        }

        #endregion

        
    }
}
