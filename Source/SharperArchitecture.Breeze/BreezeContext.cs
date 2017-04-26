using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.NH;
using Breeze.ContextProvider.NH.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Util;
using SharperArchitecture.Breeze.Events;
using SharperArchitecture.Breeze.Specification;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Extensions;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Breeze
{
    public class BreezeContext : NHContext, IBreezeContext
    {
        private readonly object _saveLock = new object();
        private readonly AsyncLock _asyncSavelock = new AsyncLock();
        private readonly IEventPublisher _eventPublisher;
        private SaveInterceptorSettings _saveInterceptorSettings;
        private AsyncSaveInterceptorSettings _asyncSaveInterceptorSettings;

        private static readonly object MetadataLock = new object();

        public BreezeContext(ISession session,  IEventPublisher eventPublisher, IBreezeConfigurator breezeConfigurator) 
            : base(session, breezeConfigurator)
        {
            _eventPublisher = eventPublisher;
        }

        protected override void CloseDbConnection()
        {
        }

        protected override string BuildJsonMetadata()
        {
            MetadataSchema meta;
            bool isBuilt;
            lock (MetadataLock)
            {
                isBuilt = IsMetadataBuilt();
                meta = GetMetadata();
            }
            if (!isBuilt)
            {
                _eventPublisher.Publish(new BreezeMetadataBuiltEvent(meta, Session.SessionFactory));
            }
            var json = JsonConvert.SerializeObject(meta, Formatting.Indented);
            return json;
        }

        public override void Dispose()
        {
        }

        #region Sync Callbacks

        public override List<EntityInfo> BeforeSaveEntityGraph(List<EntityInfo> entitiesToPersist)
        {
            _eventPublisher.Publish(new BreezeBeforeSaveEvent(entitiesToPersist));
            _saveInterceptorSettings?.BeforeSave?.Invoke(entitiesToPersist);
            return entitiesToPersist;
        }

        protected override void AfterSaveEntities(Dictionary<Type, List<EntityInfo>> saveMap, List<KeyMapping> keyMappings)
        {
            // TODO: Find a solution for async
            _eventPublisher.Publish(new BreezeAfterSaveEvent(saveMap, keyMappings));
            _saveInterceptorSettings?.AfterSave?.Invoke(saveMap, keyMappings);
            _asyncSaveInterceptorSettings?.AfterSave?.Invoke(saveMap, keyMappings);
        }

        protected override void BeforeFlush(List<EntityInfo> entitiesToPersist)
        {
            _eventPublisher.Publish(new BreezeBeforeFlushEvent(entitiesToPersist));
            _saveInterceptorSettings?.BeforeFlush?.Invoke(entitiesToPersist);
        }

        protected override void AfterFlush(List<EntityInfo> entitiesToPersist)
        {
            _eventPublisher.Publish(new BreezeAfterFlushEvent(entitiesToPersist));
            _saveInterceptorSettings?.AfterFlush?.Invoke(entitiesToPersist);
        }

        #endregion

        #region Async Callbacks

        public override async Task<List<EntityInfo>> BeforeSaveEntityGraphAsync(List<EntityInfo> entitiesToPersist)
        {
            await _eventPublisher.PublishAsync(new BreezeBeforeSaveAsyncEvent(entitiesToPersist));
            var callback = _asyncSaveInterceptorSettings?.BeforeSave;
            if (callback != null)
            {
                await callback(entitiesToPersist);
            }
            return entitiesToPersist;
        }

        protected override async Task BeforeFlushAsync(List<EntityInfo> entitiesToPersist)
        {
            await _eventPublisher.PublishAsync(new BreezeBeforeFlushAsyncEvent(entitiesToPersist));
            var callback = _asyncSaveInterceptorSettings?.BeforeFlush;
            if (callback != null)
            {
                await callback(entitiesToPersist);
            }
        }

        protected override async Task AfterFlushAsync(List<EntityInfo> entitiesToPersist)
        {
            await _eventPublisher.PublishAsync(new BreezeAfterFlushAsyncEvent(entitiesToPersist));
            var callback = _asyncSaveInterceptorSettings?.AfterFlush;
            if (callback != null)
            {
                await callback(entitiesToPersist);
            }
        }

        #endregion

        #region SaveChanges

        public SaveResult SaveChanges(JObject saveBundle)
        {
            return SaveChanges(saveBundle, null, null);
        }

        public new SaveResult SaveChanges(JObject saveBundle, TransactionSettings transactionSettings)
        {
            return SaveChanges(saveBundle, transactionSettings, null);
        }

        public SaveResult SaveChanges(JObject saveBundle, SaveInterceptorSettings interceptorSettings)
        {
            return SaveChanges(saveBundle, null, interceptorSettings);
        }

        public SaveResult SaveChanges(JObject saveBundle, TransactionSettings transactionSettings, SaveInterceptorSettings interceptorSettings)
        {
            lock (_saveLock)
            {
                _saveInterceptorSettings = interceptorSettings;
                var result = base.SaveChanges(saveBundle, transactionSettings);
                _saveInterceptorSettings = null;
                return result;
            }
        }

        #endregion

        #region SaveChangesAsync

        public Task<SaveResult> SaveChangesAsync(JObject saveBundle)
        {
            return SaveChangesAsync(saveBundle, null, null);
        }

        public new Task<SaveResult> SaveChangesAsync(JObject saveBundle, TransactionSettings transactionSettings)
        {
            return SaveChangesAsync(saveBundle, transactionSettings, null);
        }

        public Task<SaveResult> SaveChangesAsync(JObject saveBundle, AsyncSaveInterceptorSettings interceptorSettings)
        {
            return SaveChangesAsync(saveBundle, null, interceptorSettings);
        }

        public async Task<SaveResult> SaveChangesAsync(JObject saveBundle, TransactionSettings transactionSettings,
            AsyncSaveInterceptorSettings interceptorSettings)
        {
            using (await _asyncSavelock.LockAsync())
            {
                _asyncSaveInterceptorSettings = interceptorSettings;
                var result = await base.SaveChangesAsync(saveBundle, transactionSettings);
                _asyncSaveInterceptorSettings = null;
                return result;
            }
        }

        #endregion

        protected override bool HandleSaveException(Exception e, SaveWorkState saveWorkState)
        {
            var ve = e as EntityValidationException;
            if (ve == null)
                return false;
            // We need to manually rollback the transaction as we wont throw an exception
            Session.RollbackTransaction();

            var entityErrors = new List<EntityError>();
            var saveMap = saveWorkState.SaveMap;
            foreach (var vf in ve.Errors)
            {
                object[] keyValues = null;
                var entity = ve.GetEntity(vf) as IEntity;
                Type entityType = null;
                if (entity != null)
                {
                    entityType = entity.GetTypeUnproxied();
                    if (saveMap.ContainsKey(entityType))
                    {
                        var entityInfo = saveMap[entityType].FirstOrDefault(o => o.Entity == entity);
                        if (entityInfo != null)
                        {
                            keyValues = entityInfo.OriginalValuesMap.ContainsKey("id")
                                ? new[] { entityInfo.OriginalValuesMap["id"] }
                                : new[] { entity.GetId() };
                        }
                    }
                    if (keyValues == null)
                        keyValues = new[] { entity.GetId() };
                }
                entityErrors.Add(new EntityError
                {
                    EntityTypeName = entityType != null
                        ? entityType.Name
                        : ve.EntityType.Name,
                    ErrorMessage = vf.ErrorMessage,
                    ErrorName = "FluentValidationException",
                    KeyValues = keyValues,
                    PropertyName = vf.PropertyName != null
                        ? vf.PropertyName.Split('.').Last()
                        : null
                });
            }
            saveWorkState.EntityErrors = entityErrors;
            saveWorkState.KeyMappings = UpdateAutoGeneratedKeys(saveWorkState.EntitiesWithAutoGeneratedKeys);

            return true;
        }
    }
}