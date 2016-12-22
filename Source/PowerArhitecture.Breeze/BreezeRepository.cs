using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.NH;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Util;
using PowerArhitecture.Breeze.Specification;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation;

namespace PowerArhitecture.Breeze
{
    public class BreezeRepository : NHContext, IBreezeRepository
    {
        private readonly BreezeMetadataConfigurator _metadataConfigurator;
        private readonly List<IBreezeInterceptor> _interceptors;
        private readonly object _saveLock = new object();
        private readonly AsyncLock _asyncSavelock = new AsyncLock();
        private Action<List<EntityInfo>> _beforeSaveAction;
        private Func<List<EntityInfo>, Task> _beforeSaveFunc;

        public BreezeRepository(List<IBreezeInterceptor> interceptors, ISession session, BreezeMetadataConfigurator metadataConfigurator) : base(session)
        {
            _metadataConfigurator = metadataConfigurator;
            _interceptors = interceptors;
        }

        protected override void CloseDbConnection()
        {
        }

        protected override string BuildJsonMetadata()
        {
            var meta = GetMetadata();
            _metadataConfigurator.Configure(meta, Session.SessionFactory);
            var json = JsonConvert.SerializeObject(meta, Formatting.Indented);
            return json;
        }

        public override void Dispose()
        {
        }

        public override List<EntityInfo> BeforeSaveEntityGraph(List<EntityInfo> entitiesToPersist)
        {
            _beforeSaveAction?.Invoke(entitiesToPersist);
            foreach (var interceptor in _interceptors)
            {
                interceptor.BeforeSave(entitiesToPersist);
            }
            return entitiesToPersist;
        }

        public override async Task<List<EntityInfo>> BeforeSaveEntityGraphAsync(List<EntityInfo> entitiesToPersist)
        {
            if (_beforeSaveFunc != null)
            {
                await _beforeSaveFunc(entitiesToPersist);
            }
            foreach (var interceptor in _interceptors)
            {
                await interceptor.BeforeSaveAsync(entitiesToPersist);
            }
            return entitiesToPersist;
        }

        protected override void BeforeFlush(List<EntityInfo> entitiesToPersist)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.BeforeFlush(entitiesToPersist);
            }
        }

        protected override async Task BeforeFlushAsync(List<EntityInfo> entitiesToPersist)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.BeforeFlushAsync(entitiesToPersist);
            }
        }

        protected override void AfterFlush(List<EntityInfo> entitiesToPersist)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.AfterFlush(entitiesToPersist);
            }
        }

        protected override async Task AfterFlushAsync(List<EntityInfo> entitiesToPersist)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.AfterFlushAsync(entitiesToPersist);
            }
        }

        public SaveResult SaveChanges(JObject saveBundle, TransactionSettings transactionSettings, Action<List<EntityInfo>> beforeSaveAction)
        {
            lock (_saveLock)
            {
                _beforeSaveAction = beforeSaveAction;
                var result = SaveChanges(saveBundle, transactionSettings);
                _beforeSaveAction = null;
                return result;
            }
        }

        public async Task<SaveResult> SaveChangesAsync(JObject saveBundle, TransactionSettings transactionSettings, Func<List<EntityInfo>, Task> beforeSaveFunc)
        {
            using (await _asyncSavelock.LockAsync())
            {
                _beforeSaveFunc = beforeSaveFunc;
                var result =  await SaveChangesAsync(saveBundle, transactionSettings);
                _beforeSaveFunc = null;
                return result;
            }
        }

        public SaveResult SaveChanges(JObject saveBundle)
        {
            return base.SaveChanges(saveBundle);
        }

        public SaveResult SaveChanges(JObject saveBundle, Action<List<EntityInfo>> beforeSaveAction)
        {
            lock (_saveLock)
            {
                _beforeSaveAction = beforeSaveAction;
                var result = SaveChanges(saveBundle);
                _beforeSaveAction = null;
                return result;
            }
        }

        public Task<SaveResult> SaveChangesAsync(JObject saveBundle)
        {
            return base.SaveChangesAsync(saveBundle);
        }

        public async Task<SaveResult> SaveChangesAsync(JObject saveBundle, Func<List<EntityInfo>, Task> beforeSaveFunc)
        {
            using (await _asyncSavelock.LockAsync())
            {
                _beforeSaveFunc = beforeSaveFunc;
                var result = await SaveChangesAsync(saveBundle);
                _beforeSaveFunc = null;
                return result;
            }
        }

        protected override bool HandleSaveException(Exception e, SaveWorkState saveWorkState)
        {
            var ve = e as EntityValidationException;
            if (ve == null)
                return false;
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