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
using Ninject;
using Ninject.Syntax;
using PowerArhitecture.Breeze.Specification;
using PowerArhitecture.Domain;
using PowerArhitecture.Validation;

namespace PowerArhitecture.Breeze
{
    public class BreezeRepository : NHContext, IBreezeRepository
    {
        private readonly BreezeMetadataConfigurator _metadataConfigurator;
        private readonly IResolutionRoot _resolutionRoot;
        private readonly List<IBreezeInterceptor> _interceptors = new List<IBreezeInterceptor>();

        public BreezeRepository(IResolutionRoot resolutionRoot, ISession session, BreezeMetadataConfigurator metadataConfigurator) : base(session)
        {
            _metadataConfigurator = metadataConfigurator;
            _resolutionRoot = resolutionRoot;
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

        public new void Dispose()
        {
        }

        protected override Task BeforeSaveAsync(SaveWorkState saveWorkState)
        {
            _interceptors.Clear();
            var genInterceptorType = typeof(IBreezeInterceptor<>);
            foreach (var type in saveWorkState.SaveMap.Keys)
            {
                var interceptorType = genInterceptorType.MakeGenericType(type);
                var interceptor = _resolutionRoot.TryGet(interceptorType) as IBreezeInterceptor;
                if (interceptor == null) continue;
                _interceptors.Add(interceptor);
            }
            return TaskHelper.CompletedTask;
        }

        protected override Task AfterSaveAsync(SaveWorkState saveWorkState)
        {
            foreach (var interceptor in _interceptors.OfType<IDisposable>())
            {
                interceptor.Dispose();
            }
            _interceptors.Clear();
            return TaskHelper.CompletedTask;
        }

        public override async Task<List<EntityInfo>> BeforeSaveEntityGraphAsync(List<EntityInfo> entitiesToPersist)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.BeforeSave(entitiesToPersist);
            }
            return entitiesToPersist;
        }

        protected override async Task BeforeFlushAsync(List<EntityInfo> entitiesToPersist)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.BeforeFlush(entitiesToPersist);
            }
        }

        protected override async Task AfterFlushAsync(List<EntityInfo> entitiesToPersist)
        {
            foreach (var interceptor in _interceptors)
            {
                await interceptor.AfterFlush(entitiesToPersist);
            }
        }

        public Task<SaveResult> SaveChangesAsync(JObject saveBundle, Func<List<EntityInfo>, Task> beforeSaveFunc = null)
        {
            return SaveChangesAsync(saveBundle, null, beforeSaveFunc);
        }

        protected override bool HandleSaveException(Exception e, SaveWorkState saveWorkState)
        {
            var ve = e as ExtendedValidationException;
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