using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataCaching.Specifications;
using NHibernate;
using Ninject.Extensions.Logging;

namespace PowerArhitecture.DataCaching
{
    public abstract class BaseApplicationCache : IApplicationCache, 
        IListener<EntityPreInsertingEvent>,
        IListener<EntityPreUpdatingEvent>,
        IListener<EntityPreDeletingEvent>,
        IListener<EntityPreCollectionUpdatingEvent>
    {
        protected readonly ConcurrentDictionary<ISession, ConcurrentDictionary<object, object>> ModifiedObjects =
            new ConcurrentDictionary<ISession, ConcurrentDictionary<object, object>>();
        protected readonly IDataCache DataCache;
        protected readonly ILogger Logger;
        protected readonly ISessionEventListener SessionEventListener;
        protected readonly IEventAggregator EventAggregator;
        

        protected BaseApplicationCache(IEventAggregator eventAggregator, ILogger logger, IDataCache dataCache, ISessionEventListener sessionEventListener,
            IDataCachingSettings settings)
        {
            DataCache = dataCache;
            Logger = logger;
            IsMaster = settings.IsMaster;
            SessionEventListener = sessionEventListener;
            EventAggregator = eventAggregator;
            MonitoringTypes = new List<Type>();
        }

        public abstract void Initialize();

        public abstract void Refresh();

        protected IList<Type> MonitoringTypes { get; private set; }

        internal bool IsMaster { get; private set; }

        internal void BeforeInitialization()
        {
        }

        internal void AfterInitialization()
        {
        }

        #region EntityDeleted

        /// <summary>
        /// Override this method when monitoring a sub collection to return the parent
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        protected virtual object EntityPreDeleting(object entity, ISession session)
        {
            return entity;
        }

        protected virtual void EntityDeleted(object entity, ISession session)
        {
            
        }

        #endregion

        #region EntityUpdated

        /// <summary>
        /// Override this method when monitoring a sub collection to return the parent
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        protected virtual object EntityPreUpdating(object entity, ISession session)
        {
            return entity;
        }

        protected virtual void EntityUpdated(object entity, ISession session)
        {

        }

        #endregion

        #region EntityInserted

        /// <summary>
        /// Override this method when monitoring a sub collection to return the parent
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        protected virtual object EntityPreInserting(object entity, ISession session)
        {
            return entity;
        }

        protected virtual void EntityInserted(object entity, ISession session)
        {

        }

        #endregion

        #region SessionCommited

        protected virtual void SessionCommited(ISession session)
        {
            ConcurrentDictionary<object, object> dict;
            ModifiedObjects.TryRemove(session, out dict);
            foreach (var pair in dict)
            {
                var preInsert = pair.Value as EntityPreInsertingEvent;
                if (preInsert != null)
                {
                    EntityInserted(pair.Key, session);
                    continue;
                }
                var preUpdate = pair.Value as EntityPreUpdatingEvent;
                if (preUpdate != null)
                {
                    EntityUpdated(pair.Key, session);
                    continue;
                }
                var preDeleted = pair.Value as EntityPreDeletingEvent;
                if (preDeleted != null)
                {
                    EntityDeleted(pair.Key, session);
                    continue;
                }
                var preCollUpdate = pair.Value as EntityPreCollectionUpdatingEvent;
                if (preCollUpdate != null)
                {
                    EntityUpdated(pair.Key, session);
                }
            }
            dict.Clear();
        }

        #endregion


        public void Handle(EntityPreInsertingEvent message)
        {
            var @event = message.Message;
            var entity = @event.Entity;
            var session = @event.Session;
            if (MonitoringTypes.All(t => !t.IsInstanceOfType(entity))) return;
            entity = EntityPreInserting(entity, session);
            SaveModifiedObject(entity, session, message);
        }

        public void Handle(EntityPreUpdatingEvent message)
        {
            var @event = message.Message;
            var entity = @event.Entity;
            var session = @event.Session;
            if (MonitoringTypes.All(t => !t.IsInstanceOfType(entity))) return;
            entity = EntityPreUpdating(entity, @event.Session);
            SaveModifiedObject(entity, session, message);
        }

        public void Handle(EntityPreDeletingEvent message)
        {
            var @event = message.Message;
            var entity = @event.Entity;
            var session = @event.Session;
            if (MonitoringTypes.All(t => !t.IsInstanceOfType(entity))) return;
            entity = EntityPreDeleting(entity, @event.Session);
            SaveModifiedObject(entity, session, message);
        }

        public void Handle(EntityPreCollectionUpdatingEvent message)
        {
            var @event = message.Message;
            var entity = @event.AffectedOwnerOrNull;
            var session = @event.Session;
            if (ReferenceEquals(null, entity) || MonitoringTypes.All(t => !t.IsInstanceOfType(entity))) return;
            SaveModifiedObject(entity, session, message);
        }

        #region Private functions

        private void SaveModifiedObject(object entity, ISession session, object message)
        {
            if (entity == null) return;
            if (!ModifiedObjects.ContainsKey(session))
            {
                ModifiedObjects.TryAdd(session, new ConcurrentDictionary<object, object>());
                SessionEventListener.AddAListener(SessionListenerType.AfterCommit, session, () => SessionCommited(session));
            }
            var list = entity as IEnumerable; //in case user add a list add all items
            if (list != null)
            {
                foreach (var item in list)
                {
                    SaveModifiedObject(item, session, message);
                }
            }
            if (ModifiedObjects[session].ContainsKey(entity)) return;
            ModifiedObjects[session].TryAdd(entity, message);
        }

        #endregion

        
    }
}
