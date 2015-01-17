using System;
using System.Collections.Generic;
using Breeze.ContextProvider;
using Breeze.ContextProvider.NH;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Cfg;

namespace PowerArhitecture.Breeze
{
    public interface IBreezeRepository : IDisposable
    {
        object[] GetKeyValues(object entity);

        NhQueryableInclude<T> GetQuery<T>(bool cacheable = false);

        string Metadata();

        IKeyGenerator KeyGenerator { get; set; }

        SaveOptions SaveOptions { get; set; }

        SaveResult SaveChanges(JObject saveBundle, TransactionSettings transactionSettings = null);
    }
    /*
    public interface IBreezeRepository<T, TId> : IRepository<T, TId>, IBreezeRepository
        where T : class, IEntity<TId>, new()
    {
    }
    */
    public class BreezeRepository : NHContext, IBreezeRepository
    {
        private readonly BreezeMetadataConfigurator _metadataConfigurator;

        public BreezeRepository(ISession session, BreezeMetadataConfigurator metadataConfigurator) : base(session)
        {
            _metadataConfigurator = metadataConfigurator;
        }

        protected override void CloseDbConnection()
        {
        }

        protected override string BuildJsonMetadata()
        {
            var meta = GetMetadata();
            
            //new EntityErrorsException()
            _metadataConfigurator.Configure(meta, Session.SessionFactory);
            var json = JsonConvert.SerializeObject(meta, Formatting.Indented);
            return json;
        }

        public new void Dispose()
        {
        }

        protected override void OpenDbConnection()
        {
        }
    }
    /*//Problem : IRepository<T, TId> repository, ISession session can have two different session if used outside HttpContext and UnitOfWork
    public class BreezeRepository<T, TId> : BreezeRepository, IBreezeRepository<T, TId>
        where T : class, IEntity<TId>, new()
    {
        private readonly IRepository<T, TId> _repository;


        public BreezeRepository(IRepository<T, TId> repository, ISession session, Configuration configuration) : base(session, configuration)
        {
            _repository = repository;
        }

        public QueryGetEntitiesBuilder<T> GetEntitiesQuery(LockMode lockMode = LockMode.None)
        {
            return _repository.GetEntitiesQuery(lockMode);
        }

        public QueryGetEntitiesBuilder<TEntity> GetEntitiesQuery<TEntity>(LockMode lockMode = LockMode.None) where TEntity : class, IEntity, new()
        {
            return _repository.GetEntitiesQuery<TEntity>(lockMode);
        }

        public QueryGetEntityBuilder<T, TId> GetEntityQuery(TId id, LockMode lockMode = LockMode.None)
        {
            return _repository.GetEntityQuery(id, lockMode);
        }

        public IQueryable<T> GetLinqQuery()
        {
            return _repository.GetLinqQuery();
        }

        public T Get(TId id)
        {
            return _repository.Get(id);
        }

        public T Load(TId id)
        {
            return _repository.Load(id);
        }

        public void AddAListener(Action action, SessionListenerType listenerType)
        {
            _repository.AddAListener(action, listenerType);
        }

        public void AddAListener(Action<IRepository<T, TId>> action, SessionListenerType listenerType)
        {
            _repository.AddAListener(action, listenerType);
        }

        public void Save(T model)
        {
            _repository.Save(model);
        }

        public void Update(T model)
        {
            _repository.Update(model);
        }

        public void Delete(T model)
        {
            _repository.Delete(model);
        }

        public void Delete(TId id)
        {
            _repository.Delete(id);
        }

        public T DeepCopy(T model)
        {
            return _repository.DeepCopy(model);
        }

        public IEnumerable<PropertyInfo> GetMappedProperties()
        {
            return _repository.GetMappedProperties();
        }
    }*/
}