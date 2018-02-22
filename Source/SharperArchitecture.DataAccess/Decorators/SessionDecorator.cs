using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Loader.Custom;
using NHibernate.Persister.Entity;
using NHibernate.Stat;
using NHibernate.Transaction;
using NHibernate.Type;
using SharperArchitecture.Common.Events;
using SharperArchitecture.DataAccess.Events;
using SharperArchitecture.DataAccess.Providers;
using SimpleInjector;

namespace SharperArchitecture.DataAccess.Decorators
{
    /// <summary>
    /// A session decorator that enables the session to be lazily injected
    /// </summary>
    internal class SessionDecorator : IEventSource
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Lazy<IEventSource> _lazyEventSource;

        public SessionDecorator(Lazy<IEventSource> lazyEventSource, IEventPublisher eventPublisher, string dbConfigName)
        {
            _lazyEventSource = lazyEventSource;
            _eventPublisher = eventPublisher;
            DatabaseConfigurationName = dbConfigName;
        }

        public string DatabaseConfigurationName { get; }

        internal IEventSource Session
        {
            get
            {
                if (IsDisposed && !_lazyEventSource.IsValueCreated)
                {
                    throw new ActivationException("Cannot initialize ISession outside the scope that was created");
                }
                return _lazyEventSource.Value;
            }
        }


        internal bool IsDisposed { get; private set; }

        public Task<IQuery> CreateFilterAsync(object collection, IQueryExpression queryExpression, CancellationToken cancellationToken)
        {
            return Session.CreateFilterAsync(collection, queryExpression, cancellationToken);
        }

        public void Initialize()
        {
            Session.Initialize();
        }

        public Task InitializeCollectionAsync(IPersistentCollection collection, bool writing, CancellationToken cancellationToken)
        {
            return Session.InitializeCollectionAsync(collection, writing, cancellationToken);
        }

        public void InitializeCollection(IPersistentCollection collection, bool writing)
        {
            Session.InitializeCollection(collection, writing);
        }

        public object InternalLoad(string entityName, object id, bool eager, bool isNullable)
        {
            return Session.InternalLoad(entityName, id, eager, isNullable);
        }

        public Task<object> InternalLoadAsync(string entityName, object id, bool eager, bool isNullable, CancellationToken cancellationToken)
        {
            return Session.InternalLoadAsync(entityName, id, eager, isNullable, cancellationToken);
        }

        public object ImmediateLoad(string entityName, object id)
        {
            return Session.ImmediateLoad(entityName, id);
        }

        public Task<object> ImmediateLoadAsync(string entityName, object id, CancellationToken cancellationToken)
        {
            return Session.ImmediateLoadAsync(entityName, id, cancellationToken);
        }

        public IList List(IQueryExpression queryExpression, QueryParameters parameters)
        {
            return Session.List(queryExpression, parameters);
        }

        public Task<IList> ListAsync(IQueryExpression queryExpression, QueryParameters parameters, CancellationToken cancellationToken)
        {
            return Session.ListAsync(queryExpression, parameters, cancellationToken);
        }

        public IQuery CreateQuery(IQueryExpression queryExpression)
        {
            return Session.CreateQuery(queryExpression);
        }

        public void List(IQueryExpression queryExpression, QueryParameters queryParameters, IList results)
        {
            Session.List(queryExpression, queryParameters, results);
        }

        public Task ListAsync(IQueryExpression queryExpression, QueryParameters queryParameters, IList results, CancellationToken cancellationToken)
        {
            return Session.ListAsync(queryExpression, queryParameters, results, cancellationToken);
        }

        public IList<T> List<T>(IQueryExpression queryExpression, QueryParameters queryParameters)
        {
            return Session.List<T>(queryExpression, queryParameters);
        }

        public Task<IList<T>> ListAsync<T>(IQueryExpression queryExpression, QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            return Session.ListAsync<T>(queryExpression, queryParameters, cancellationToken);
        }

        public IList<T> List<T>(CriteriaImpl criteria)
        {
            return Session.List<T>(criteria);
        }

        public Task<IList<T>> ListAsync<T>(CriteriaImpl criteria, CancellationToken cancellationToken)
        {
            return Session.ListAsync<T>(criteria, cancellationToken);
        }

        public void List(CriteriaImpl criteria, IList results)
        {
            Session.List(criteria, results);
        }

        public Task ListAsync(CriteriaImpl criteria, IList results, CancellationToken cancellationToken)
        {
            return Session.ListAsync(criteria, results, cancellationToken);
        }

        public IList List(CriteriaImpl criteria)
        {
            return Session.List(criteria);
        }

        public Task<IList> ListAsync(CriteriaImpl criteria, CancellationToken cancellationToken)
        {
            return Session.ListAsync(criteria, cancellationToken);
        }

        public IEnumerable Enumerable(IQueryExpression query, QueryParameters parameters)
        {
            return Session.Enumerable(query, parameters);
        }

        public Task<IEnumerable> EnumerableAsync(IQueryExpression query, QueryParameters parameters, CancellationToken cancellationToken)
        {
            return Session.EnumerableAsync(query, parameters, cancellationToken);
        }

        public IEnumerable<T> Enumerable<T>(IQueryExpression query, QueryParameters queryParameters)
        {
            return Session.Enumerable<T>(query, queryParameters);
        }

        public Task<IEnumerable<T>> EnumerableAsync<T>(IQueryExpression query, QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            return Session.EnumerableAsync<T>(query, queryParameters, cancellationToken);
        }

        public IList ListFilter(object collection, string filter, QueryParameters parameters)
        {
            return Session.ListFilter(collection, filter, parameters);
        }

        public IList ListFilter(object collection, IQueryExpression queryExpression, QueryParameters parameters)
        {
            return Session.ListFilter(collection, queryExpression, parameters);
        }

        public Task<IList> ListFilterAsync(object collection, string filter, QueryParameters parameters, CancellationToken cancellationToken)
        {
            return Session.ListFilterAsync(collection, filter, parameters, cancellationToken);
        }

        public Task<IList> ListFilterAsync(object collection, IQueryExpression queryExpression, QueryParameters parameters,
            CancellationToken cancellationToken)
        {
            return Session.ListFilterAsync(collection, queryExpression, parameters, cancellationToken);
        }

        public IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters)
        {
            return Session.ListFilter<T>(collection, filter, parameters);
        }

        public Task<IList<T>> ListFilterAsync<T>(object collection, string filter, QueryParameters parameters, CancellationToken cancellationToken)
        {
            return Session.ListFilterAsync<T>(collection, filter, parameters, cancellationToken);
        }

        public IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
        {
            return Session.EnumerableFilter(collection, filter, parameters);
        }

        public Task<IEnumerable> EnumerableFilterAsync(object collection, string filter, QueryParameters parameters, CancellationToken cancellationToken)
        {
            return Session.EnumerableFilterAsync(collection, filter, parameters, cancellationToken);
        }

        public IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
        {
            return Session.EnumerableFilter<T>(collection, filter, parameters);
        }

        public Task<IEnumerable<T>> EnumerableFilterAsync<T>(object collection, string filter,
            QueryParameters parameters, CancellationToken cancellationToken)
        {
            return Session.EnumerableFilterAsync<T>(collection, filter, parameters, cancellationToken);
        }

        public Task BeforeTransactionCompletionAsync(ITransaction tx, CancellationToken cancellationToken)
        {
            return Session.BeforeTransactionCompletionAsync(tx, cancellationToken);
        }

        public Task FlushBeforeTransactionCompletionAsync(CancellationToken cancellationToken)
        {
            return Session.FlushBeforeTransactionCompletionAsync(cancellationToken);
        }

        public Task AfterTransactionCompletionAsync(bool successful, ITransaction tx, CancellationToken cancellationToken)
        {
            return Session.AfterTransactionCompletionAsync(successful, tx, cancellationToken);
        }

        public IEntityPersister GetEntityPersister(string entityName, object obj)
        {
            return Session.GetEntityPersister(entityName, obj);
        }

        public void AfterTransactionBegin(ITransaction tx)
        {
            Session.AfterTransactionBegin(tx);
        }

        public void BeforeTransactionCompletion(ITransaction tx)
        {
            Session.BeforeTransactionCompletion(tx);
        }

        public void FlushBeforeTransactionCompletion()
        {
            Session.FlushBeforeTransactionCompletion();
        }

        public void AfterTransactionCompletion(bool successful, ITransaction tx)
        {
            Session.AfterTransactionCompletion(successful, tx);
        }

        public object GetContextEntityIdentifier(object obj)
        {
            return Session.GetContextEntityIdentifier(obj);
        }

        public object Instantiate(string entityName, object id)
        {
            return Session.Instantiate(entityName, id);
        }

        public IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
        {
            return Session.List(spec, queryParameters);
        }

        public Task<IList> ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            return Session.ListAsync(spec, queryParameters, cancellationToken);
        }

        public void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
        {
            Session.List(spec, queryParameters, results);
        }

        public Task ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results, CancellationToken cancellationToken)
        {
            return Session.ListAsync(spec, queryParameters, results, cancellationToken);
        }

        public IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
        {
            return Session.List<T>(spec, queryParameters);
        }

        public Task<IList<T>> ListAsync<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            return Session.ListAsync<T>(spec, queryParameters, cancellationToken);
        }

        public void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
        {
            Session.ListCustomQuery(customQuery, queryParameters, results);
        }

        public Task ListCustomQueryAsync(ICustomQuery customQuery, QueryParameters queryParameters, IList results, CancellationToken cancellationToken)
        {
            return Session.ListCustomQueryAsync(customQuery, queryParameters, results, cancellationToken);
        }

        public IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
        {
            return Session.ListCustomQuery<T>(customQuery, queryParameters);
        }

        public Task<IList<T>> ListCustomQueryAsync<T>(ICustomQuery customQuery, QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            return Session.ListCustomQueryAsync<T>(customQuery, queryParameters, cancellationToken);
        }

        public Task<IQueryTranslator[]> GetQueriesAsync(IQueryExpression query, bool scalar, CancellationToken cancellationToken)
        {
            return Session.GetQueriesAsync(query, scalar, cancellationToken);
        }

        public object GetFilterParameterValue(string filterParameterName)
        {
            return Session.GetFilterParameterValue(filterParameterName);
        }

        public IType GetFilterParameterType(string filterParameterName)
        {
            return Session.GetFilterParameterType(filterParameterName);
        }

        public IQuery GetNamedSQLQuery(string name)
        {
            return Session.GetNamedSQLQuery(name);
        }

        public IQueryTranslator[] GetQueries(IQueryExpression query, bool scalar)
        {
            return Session.GetQueries(query, scalar);
        }

        public Task<object> GetEntityUsingInterceptorAsync(EntityKey key, CancellationToken cancellationToken)
        {
            return Session.GetEntityUsingInterceptorAsync(key, cancellationToken);
        }

        public object GetEntityUsingInterceptor(EntityKey key)
        {
            return Session.GetEntityUsingInterceptor(key);
        }

        public string BestGuessEntityName(object entity)
        {
            return Session.BestGuessEntityName(entity);
        }

        public string GuessEntityName(object entity)
        {
            return Session.GuessEntityName(entity);
        }

        public Task<IQuery> CreateFilterAsync(object collection, string queryString, CancellationToken cancellationToken)
        {
            return Session.CreateFilterAsync(collection, queryString, cancellationToken);
        }

        IQuery ISession.GetNamedQuery(string queryName)
        {
            return ((ISession) Session).GetNamedQuery(queryName);
        }

        public ISQLQuery CreateSQLQuery(string queryString)
        {
            return Session.CreateSQLQuery(queryString);
        }

        public void Clear()
        {
            Session.Clear();
        }

        public object Get(Type clazz, object id)
        {
            return Session.Get(clazz, id);
        }

        public Task<object> GetAsync(Type clazz, object id, CancellationToken cancellationToken)
        {
            return Session.GetAsync(clazz, id, cancellationToken);
        }

        public object Get(Type clazz, object id, LockMode lockMode)
        {
            return Session.Get(clazz, id, lockMode);
        }

        public Task<object> GetAsync(Type clazz, object id, LockMode lockMode, CancellationToken cancellationToken)
        {
            return Session.GetAsync(clazz, id, lockMode, cancellationToken);
        }

        public object Get(string entityName, object id)
        {
            return Session.Get(entityName, id);
        }

        public Task<object> GetAsync(string entityName, object id, CancellationToken cancellationToken)
        {
            return Session.GetAsync(entityName, id, cancellationToken);
        }

        public T Get<T>(object id)
        {
            return Session.Get<T>(id);
        }

        public Task<T> GetAsync<T>(object id, CancellationToken cancellationToken)
        {
            return Session.GetAsync<T>(id, cancellationToken);
        }

        public T Get<T>(object id, LockMode lockMode)
        {
            return Session.Get<T>(id, lockMode);
        }

        public Task<T> GetAsync<T>(object id, LockMode lockMode, CancellationToken cancellationToken)
        {
            return Session.GetAsync<T>(id, lockMode, cancellationToken);
        }

        public string GetEntityName(object obj)
        {
            return Session.GetEntityName(obj);
        }

        public Task<string> GetEntityNameAsync(object obj, CancellationToken cancellationToken)
        {
            return Session.GetEntityNameAsync(obj, cancellationToken);
        }

        public ISharedSessionBuilder SessionWithOptions()
        {
            return Session.SessionWithOptions();
        }

        public IFilter EnableFilter(string filterName)
        {
            return Session.EnableFilter(filterName);
        }

        public IFilter GetEnabledFilter(string filterName)
        {
            return Session.GetEnabledFilter(filterName);
        }

        public void DisableFilter(string filterName)
        {
            Session.DisableFilter(filterName);
        }

        public IMultiQuery CreateMultiQuery()
        {
            return Session.CreateMultiQuery();
        }

        public ISession SetBatchSize(int batchSize)
        {
            return Session.SetBatchSize(batchSize);
        }

        public ISessionImplementor GetSessionImplementation()
        {
            return this;
        }

        public IMultiCriteria CreateMultiCriteria()
        {
            return Session.CreateMultiCriteria();
        }

        public ISession GetSession(EntityMode entityMode)
        {
#pragma warning disable 618
            return Session.GetSession(entityMode);
#pragma warning restore 618
        }

        public IQueryable<T> Query<T>()
        {
            return Session.Query<T>();
        }

        public IQueryable<T> Query<T>(string entityName)
        {
            return Session.Query<T>(entityName);
        }

        FlushMode ISession.FlushMode
        {
            get => ((ISession) Session).FlushMode;
            set => ((ISession) Session).FlushMode = value;
        }

        CacheMode ISession.CacheMode
        {
            get => ((ISession) Session).CacheMode;
            set => ((ISession) Session).CacheMode = value;
        }

        IQuery ISessionImplementor.GetNamedQuery(string queryName)
        {
            return ((ISessionImplementor) Session).GetNamedQuery(queryName);
        }

        void ISessionImplementor.Flush()
        {
            _eventPublisher.Publish(new SessionFlushingEvent(Session));
            ((ISessionImplementor) Session).Flush();
            _eventPublisher.Publish(new SessionFlushedEvent(Session));
        }

        async Task ISession.FlushAsync(CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new SessionFlushingAsyncEvent(Session), cancellationToken);
            await ((ISession) Session).FlushAsync(cancellationToken);
            await _eventPublisher.PublishAsync(new SessionFlushedAsyncEvent(Session), cancellationToken);
        }

        public DbConnection Disconnect()
        {
            return Session.Disconnect();
        }

        public void Reconnect()
        {
            Session.Reconnect();
        }

        public void Reconnect(DbConnection connection)
        {
            Session.Reconnect(connection);
        }

        public DbConnection Close()
        {
            return Session.Close();
        }

        public void CancelQuery()
        {
            Session.CancelQuery();
        }

        public bool IsDirty()
        {
            return Session.IsDirty();
        }

        public Task<bool> IsDirtyAsync(CancellationToken cancellationToken)
        {
            return Session.IsDirtyAsync(cancellationToken);
        }

        public bool IsReadOnly(object entityOrProxy)
        {
            return Session.IsReadOnly(entityOrProxy);
        }

        public void SetReadOnly(object entityOrProxy, bool readOnly)
        {
            Session.SetReadOnly(entityOrProxy, readOnly);
        }

        public object GetIdentifier(object obj)
        {
            return Session.GetIdentifier(obj);
        }

        public bool Contains(object obj)
        {
            return Session.Contains(obj);
        }

        public void Evict(object obj)
        {
            Session.Evict(obj);
        }

        public Task EvictAsync(object obj, CancellationToken cancellationToken)
        {
            return Session.EvictAsync(obj, cancellationToken);
        }

        public object Load(Type theType, object id, LockMode lockMode)
        {
            return Session.Load(theType, id, lockMode);
        }

        public Task<object> LoadAsync(Type theType, object id, LockMode lockMode, CancellationToken cancellationToken)
        {
            return Session.LoadAsync(theType, id, lockMode, cancellationToken);
        }

        public object Load(string entityName, object id, LockMode lockMode)
        {
            return Session.Load(entityName, id, lockMode);
        }

        public Task<object> LoadAsync(string entityName, object id, LockMode lockMode, CancellationToken cancellationToken)
        {
            return Session.LoadAsync(entityName, id, lockMode, cancellationToken);
        }

        public object Load(Type theType, object id)
        {
            return Session.Load(theType, id);
        }

        public Task<object> LoadAsync(Type theType, object id, CancellationToken cancellationToken)
        {
            return Session.LoadAsync(theType, id, cancellationToken);
        }

        public T Load<T>(object id, LockMode lockMode)
        {
            return Session.Load<T>(id, lockMode);
        }

        public Task<T> LoadAsync<T>(object id, LockMode lockMode, CancellationToken cancellationToken)
        {
            return Session.LoadAsync<T>(id, lockMode, cancellationToken);
        }

        public T Load<T>(object id)
        {
            return Session.Load<T>(id);
        }

        public Task<T> LoadAsync<T>(object id, CancellationToken cancellationToken)
        {
            return Session.LoadAsync<T>(id, cancellationToken);
        }

        public object Load(string entityName, object id)
        {
            return Session.Load(entityName, id);
        }

        public Task<object> LoadAsync(string entityName, object id, CancellationToken cancellationToken)
        {
            return Session.LoadAsync(entityName, id, cancellationToken);
        }

        public void Load(object obj, object id)
        {
            Session.Load(obj, id);
        }

        public Task LoadAsync(object obj, object id, CancellationToken cancellationToken)
        {
            return Session.LoadAsync(obj, id, cancellationToken);
        }

        public void Replicate(object obj, ReplicationMode replicationMode)
        {
            Session.Replicate(obj, replicationMode);
        }

        public Task ReplicateAsync(object obj, ReplicationMode replicationMode, CancellationToken cancellationToken)
        {
            return Session.ReplicateAsync(obj, replicationMode, cancellationToken);
        }

        public void Replicate(string entityName, object obj, ReplicationMode replicationMode)
        {
            Session.Replicate(entityName, obj, replicationMode);
        }

        public Task ReplicateAsync(string entityName, object obj, ReplicationMode replicationMode, CancellationToken cancellationToken)
        {
            return Session.ReplicateAsync(entityName, obj, replicationMode, cancellationToken);
        }

        public object Save(object obj)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            return Session.Save(obj);
        }

        public async Task<object> SaveAsync(object obj, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            return await Session.SaveAsync(obj, cancellationToken);
        }

        public void Save(object obj, object id)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            Session.Save(obj, id);
        }

        public async Task SaveAsync(object obj, object id, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            await Session.SaveAsync(obj, id, cancellationToken);
        }

        public object Save(string entityName, object obj)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            return Session.Save(entityName, obj);
        }

        public async Task<object> SaveAsync(string entityName, object obj, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            return await Session.SaveAsync(entityName, obj, cancellationToken);
        }

        public void Save(string entityName, object obj, object id)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            Session.Save(entityName, obj, id);
        }

        public async Task SaveAsync(string entityName, object obj, object id, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            await Session.SaveAsync(entityName, obj, id, cancellationToken);
        }

        public void SaveOrUpdate(object obj)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            Session.SaveOrUpdate(obj);
        }

        public async Task SaveOrUpdateAsync(object obj, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            await Session.SaveOrUpdateAsync(obj, cancellationToken);
        }

        public void SaveOrUpdate(string entityName, object obj)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            Session.SaveOrUpdate(entityName, obj);
        }

        public async Task SaveOrUpdateAsync(string entityName, object obj, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            await Session.SaveOrUpdateAsync(entityName, obj, cancellationToken);
        }

        public void SaveOrUpdate(string entityName, object obj, object id)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            Session.SaveOrUpdate(entityName, obj, id);
        }

        public async Task SaveOrUpdateAsync(string entityName, object obj, object id, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            await Session.SaveOrUpdateAsync(entityName, obj, id, cancellationToken);
        }

        public void Update(object obj)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            Session.Update(obj);
        }

        public async Task UpdateAsync(object obj, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            await Session.UpdateAsync(obj, cancellationToken);
        }

        public void Update(object obj, object id)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            Session.Update(obj, id);
        }

        public async Task UpdateAsync(object obj, object id, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            await Session.UpdateAsync(obj, id, cancellationToken);
        }

        public void Update(string entityName, object obj)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            Session.Update(entityName, obj);
        }

        public async Task UpdateAsync(string entityName, object obj, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            await Session.UpdateAsync(entityName, obj, cancellationToken);
        }

        public void Update(string entityName, object obj, object id)
        {
            _eventPublisher.Publish(new EntitySavingOrUpdatingEvent(Session));
            Session.Update(entityName, obj, id);
        }

        public async Task UpdateAsync(string entityName, object obj, object id, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntitySavingOrUpdatingAsyncEvent(Session), cancellationToken);
            await Session.UpdateAsync(entityName, obj, id, cancellationToken);
        }

        public object Merge(object obj)
        {
            return Session.Merge(obj);
        }

        public Task<object> MergeAsync(object obj, CancellationToken cancellationToken)
        {
            return Session.MergeAsync(obj, cancellationToken);
        }

        public object Merge(string entityName, object obj)
        {
            return Session.Merge(entityName, obj);
        }

        public Task<object> MergeAsync(string entityName, object obj, CancellationToken cancellationToken)
        {
            return Session.MergeAsync(entityName, obj, cancellationToken);
        }

        public T Merge<T>(T entity) where T : class
        {
            return Session.Merge(entity);
        }

        public Task<T> MergeAsync<T>(T entity, CancellationToken cancellationToken) where T : class
        {
            return Session.MergeAsync(entity, cancellationToken);
        }

        public T Merge<T>(string entityName, T entity) where T : class
        {
            return Session.Merge(entityName, entity);
        }

        public Task<T> MergeAsync<T>(string entityName, T entity, CancellationToken cancellationToken) where T : class
        {
            return Session.MergeAsync(entityName, entity, cancellationToken);
        }

        public void Persist(object obj)
        {
            Session.Persist(obj);
        }

        public Task PersistAsync(object obj, CancellationToken cancellationToken)
        {
            return Session.PersistAsync(obj, cancellationToken);
        }

        public void Persist(string entityName, object obj)
        {
            Session.Persist(entityName, obj);
        }

        public Task PersistAsync(string entityName, object obj, CancellationToken cancellationToken)
        {
            return Session.PersistAsync(entityName, obj, cancellationToken);
        }

        public void Delete(object obj)
        {
            _eventPublisher.Publish(new EntityDeletingEvent(Session));
            Session.Delete(obj);
        }

        public async Task DeleteAsync(object obj, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntityDeletingAsyncEvent(Session), cancellationToken);
            await Session.DeleteAsync(obj, cancellationToken);
        }

        public void Delete(string entityName, object obj)
        {
            _eventPublisher.Publish(new EntityDeletingEvent(Session));
            Session.Delete(entityName, obj);
        }

        public async Task DeleteAsync(string entityName, object obj, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntityDeletingAsyncEvent(Session), cancellationToken);
            await Session.DeleteAsync(entityName, obj, cancellationToken);
        }

        public int Delete(string query)
        {
            _eventPublisher.Publish(new EntityDeletingEvent(Session));
            return Session.Delete(query);
        }

        public async Task<int> DeleteAsync(string query, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntityDeletingAsyncEvent(Session), cancellationToken);
            return await Session.DeleteAsync(query, cancellationToken);
        }

        public int Delete(string query, object value, IType type)
        {
            _eventPublisher.Publish(new EntityDeletingEvent(Session));
            return Session.Delete(query, value, type);
        }

        public async Task<int> DeleteAsync(string query, object value, IType type, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntityDeletingAsyncEvent(Session), cancellationToken);
            return await Session.DeleteAsync(query, value, type, cancellationToken);
        }

        public int Delete(string query, object[] values, IType[] types)
        {
            _eventPublisher.Publish(new EntityDeletingEvent(Session));
            return Session.Delete(query, values, types);
        }

        public async Task<int> DeleteAsync(string query, object[] values, IType[] types, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntityDeletingAsyncEvent(Session), cancellationToken);
            return await Session.DeleteAsync(query, values, types, cancellationToken);
        }

        public void Lock(object obj, LockMode lockMode)
        {
            Session.Lock(obj, lockMode);
        }

        public Task LockAsync(object obj, LockMode lockMode, CancellationToken cancellationToken)
        {
            return Session.LockAsync(obj, lockMode, cancellationToken);
        }

        public void Lock(string entityName, object obj, LockMode lockMode)
        {
            Session.Lock(entityName, obj, lockMode);
        }

        public Task LockAsync(string entityName, object obj, LockMode lockMode, CancellationToken cancellationToken)
        {
            return Session.LockAsync(entityName, obj, lockMode, cancellationToken);
        }

        public void Refresh(object obj)
        {
            Session.Refresh(obj);
        }

        public Task RefreshAsync(object obj, CancellationToken cancellationToken)
        {
            return Session.RefreshAsync(obj, cancellationToken);
        }

        public void Refresh(object obj, LockMode lockMode)
        {
            Session.Refresh(obj, lockMode);
        }

        public Task RefreshAsync(object obj, LockMode lockMode, CancellationToken cancellationToken)
        {
            return Session.RefreshAsync(obj, lockMode, cancellationToken);
        }

        public LockMode GetCurrentLockMode(object obj)
        {
            return Session.GetCurrentLockMode(obj);
        }

        public ITransaction BeginTransaction()
        {
            return Session.BeginTransaction();
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return Session.BeginTransaction(isolationLevel);
        }

        void ISession.JoinTransaction()
        {
            ((ISession) Session).JoinTransaction();
        }

        public ICriteria CreateCriteria<T>() where T : class
        {
            return Session.CreateCriteria<T>();
        }

        public ICriteria CreateCriteria<T>(string alias) where T : class
        {
            return Session.CreateCriteria<T>(alias);
        }

        public ICriteria CreateCriteria(Type persistentClass)
        {
            return Session.CreateCriteria(persistentClass);
        }

        public ICriteria CreateCriteria(Type persistentClass, string alias)
        {
            return Session.CreateCriteria(persistentClass, alias);
        }

        public ICriteria CreateCriteria(string entityName)
        {
            return Session.CreateCriteria(entityName);
        }

        public ICriteria CreateCriteria(string entityName, string alias)
        {
            return Session.CreateCriteria(entityName, alias);
        }

        public IQueryOver<T, T> QueryOver<T>() where T : class
        {
            return Session.QueryOver<T>();
        }

        public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class
        {
            return Session.QueryOver(alias);
        }

        public IQueryOver<T, T> QueryOver<T>(string entityName) where T : class
        {
            return Session.QueryOver<T>(entityName);
        }

        public IQueryOver<T, T> QueryOver<T>(string entityName, Expression<Func<T>> alias) where T : class
        {
            return Session.QueryOver(entityName, alias);
        }

        public IQuery CreateQuery(string queryString)
        {
            return Session.CreateQuery(queryString);
        }

        public IQuery CreateFilter(object collection, string queryString)
        {
            return Session.CreateFilter(collection, queryString);
        }

        void ISession.Flush()
        {
            _eventPublisher.Publish(new SessionFlushingEvent(Session));
            ((ISession) Session).Flush();
            _eventPublisher.Publish(new SessionFlushedEvent(Session));
        }

        async Task ISessionImplementor.FlushAsync(CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new SessionFlushingAsyncEvent(Session), cancellationToken);
            await ((ISessionImplementor) Session).FlushAsync(cancellationToken);
            await _eventPublisher.PublishAsync(new SessionFlushedAsyncEvent(Session), cancellationToken);
        }

        public int ExecuteNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters)
        {
            return Session.ExecuteNativeUpdate(specification, queryParameters);
        }

        public Task<int> ExecuteNativeUpdateAsync(NativeSQLQuerySpecification specification,
            QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            return Session.ExecuteNativeUpdateAsync(specification, queryParameters, cancellationToken);
        }

        public int ExecuteUpdate(IQueryExpression query, QueryParameters queryParameters)
        {
            return Session.ExecuteUpdate(query, queryParameters);
        }

        void ISessionImplementor.JoinTransaction()
        {
            ((ISessionImplementor) Session).JoinTransaction();
        }

        public void CloseSessionFromSystemTransaction()
        {
            Session.CloseSessionFromSystemTransaction();
        }

        public IQuery CreateFilter(object collection, IQueryExpression queryExpression)
        {
            return Session.CreateFilter(collection, queryExpression);
        }

        public Task<int> ExecuteUpdateAsync(IQueryExpression query, QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            return Session.ExecuteUpdateAsync(query, queryParameters, cancellationToken);
        }

        public EntityKey GenerateEntityKey(object id, IEntityPersister persister)
        {
            return Session.GenerateEntityKey(id, persister);
        }

        public CacheKey GenerateCacheKey(object id, IType type, string entityOrRoleName)
        {
            return Session.GenerateCacheKey(id, type, entityOrRoleName);
        }

        public long Timestamp => Session.Timestamp;

        public ISessionFactoryImplementor Factory => Session.Factory;

        public IBatcher Batcher => Session.Batcher;

        public IDictionary<string, IFilter> EnabledFilters => Session.EnabledFilters;

        public IInterceptor Interceptor => Session.Interceptor;

        public NHibernate.Event.EventListeners Listeners => Session.Listeners;

        public ConnectionManager ConnectionManager => Session.ConnectionManager;

        public bool IsEventSource => Session.IsEventSource;

        public IPersistenceContext PersistenceContext => Session.PersistenceContext;

        CacheMode ISessionImplementor.CacheMode
        {
            get => ((ISessionImplementor) Session).CacheMode;
            set => ((ISessionImplementor) Session).CacheMode = value;
        }

        DbConnection ISession.Connection => ((ISession) Session).Connection;

        bool ISession.IsOpen => ((ISession) Session).IsOpen;

        bool ISession.IsConnected => ((ISession) Session).IsConnected;

        public bool DefaultReadOnly
        {
            get => Session.DefaultReadOnly;
            set => Session.DefaultReadOnly = value;
        }

        public ITransaction Transaction => Session.Transaction;

        public ISessionStatistics Statistics => Session.Statistics;

        bool ISessionImplementor.IsOpen => ((ISessionImplementor) Session).IsOpen;

        bool ISessionImplementor.IsConnected => ((ISessionImplementor) Session).IsConnected;

        FlushMode ISessionImplementor.FlushMode
        {
            get => ((ISessionImplementor) Session).FlushMode;
            set => ((ISessionImplementor) Session).FlushMode = value;
        }

        public string FetchProfile
        {
            get => Session.FetchProfile;
            set => Session.FetchProfile = value;
        }

        public ISessionFactory SessionFactory => Session.SessionFactory;

        DbConnection ISessionImplementor.Connection => ((ISessionImplementor) Session).Connection;

        public bool IsClosed => Session.IsClosed;

        public bool TransactionInProgress => Session.TransactionInProgress;

        public FutureCriteriaBatch FutureCriteriaBatch => Session.FutureCriteriaBatch;

        public FutureQueryBatch FutureQueryBatch => Session.FutureQueryBatch;

        public Guid SessionId => Session.SessionId;

        public ITransactionContext TransactionContext
        {
            get => Session.TransactionContext;
            set => Session.TransactionContext = value;
        }

        public void Dispose()
        {
            IsDisposed = true;
            if (!_lazyEventSource.IsValueCreated)
            {
                return;
            }
            Session.Dispose();
            SessionProvider.RegisteredSessionIds.TryRemove(Session.SessionId);
        }

        public object Instantiate(IEntityPersister persister, object id)
        {
            return Session.Instantiate(persister, id);
        }

        public void ForceFlush(EntityEntry e)
        {
            Session.ForceFlush(e);
        }

        public void Merge(string entityName, object obj, IDictionary copiedAlready)
        {
            Session.Merge(entityName, obj, copiedAlready);
        }

        public void Persist(string entityName, object obj, IDictionary createdAlready)
        {
            Session.Persist(entityName, obj, createdAlready);
        }

        public void PersistOnFlush(string entityName, object obj, IDictionary copiedAlready)
        {
            Session.PersistOnFlush(entityName, obj, copiedAlready);
        }

        public void Refresh(object obj, IDictionary refreshedAlready)
        {
            Session.Refresh(obj, refreshedAlready);
        }

        public void Delete(string entityName, object child, bool isCascadeDeleteEnabled, ISet<object> transientEntities)
        {
            Session.Delete(entityName, child, isCascadeDeleteEnabled, transientEntities);
        }

        public IDisposable SuspendAutoFlush()
        {
            return Session.SuspendAutoFlush();
        }

        public async Task ForceFlushAsync(EntityEntry e, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new SessionFlushingAsyncEvent(Session), cancellationToken);
            await Session.ForceFlushAsync(e, cancellationToken);
            await _eventPublisher.PublishAsync(new SessionFlushedAsyncEvent(Session), cancellationToken);
        }

        public Task MergeAsync(string entityName, object obj, IDictionary copiedAlready, CancellationToken cancellationToken)
        {
            return Session.MergeAsync(entityName, obj, copiedAlready, cancellationToken);
        }

        public Task PersistAsync(string entityName, object obj, IDictionary createdAlready, CancellationToken cancellationToken)
        {
            return Session.PersistAsync(entityName, obj, createdAlready, cancellationToken);
        }

        public Task PersistOnFlushAsync(string entityName, object obj, IDictionary copiedAlready, CancellationToken cancellationToken)
        {
            return Session.PersistOnFlushAsync(entityName, obj, copiedAlready, cancellationToken);
        }

        public Task RefreshAsync(object obj, IDictionary refreshedAlready, CancellationToken cancellationToken)
        {
            return Session.RefreshAsync(obj, refreshedAlready, cancellationToken);
        }

        public async Task DeleteAsync(string entityName, object child, bool isCascadeDeleteEnabled,
            ISet<object> transientEntities, CancellationToken cancellationToken)
        {
            await _eventPublisher.PublishAsync(new EntityDeletingAsyncEvent(Session), cancellationToken);
            await Session.DeleteAsync(entityName, child, isCascadeDeleteEnabled, transientEntities, cancellationToken);
        }

        public ActionQueue ActionQueue => Session.ActionQueue;
        public bool AutoFlushSuspended => Session.AutoFlushSuspended;


        public override int GetHashCode()
        {
            return Session.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Session.Equals(obj);
        }
    }
}
