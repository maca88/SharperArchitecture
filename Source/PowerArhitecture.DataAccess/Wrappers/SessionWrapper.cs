using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;

namespace PowerArhitecture.DataAccess.Wrappers
{
    public class SessionWrapper : IEventSource
    {
        private readonly IEventSource _session;
        private readonly IEventAggregator _eventAggregator;

        public SessionWrapper(IEventSource session, IEventAggregator eventAggregator)
        {
            _session = session;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _session.Initialize();
        }

        public Task InitializeCollection(IPersistentCollection collection, bool writing)
        {
            return _session.InitializeCollection(collection, writing);
        }

        public object InternalLoad(string entityName, object id, bool eager, bool isNullable)
        {
            return _session.InternalLoad(entityName, id, eager, isNullable);
        }

        public Task<object> InternalLoadAsync(string entityName, object id, bool eager, bool isNullable)
        {
            return _session.InternalLoadAsync(entityName, id, eager, isNullable);
        }

        public object ImmediateLoad(string entityName, object id)
        {
            return _session.ImmediateLoad(entityName, id);
        }

        public Task<object> ImmediateLoadAsync(string entityName, object id)
        {
            return _session.ImmediateLoadAsync(entityName, id);
        }

        public IList List(string query, QueryParameters parameters)
        {
            return _session.List(query, parameters);
        }

        public IList List(IQueryExpression queryExpression, QueryParameters parameters)
        {
            return _session.List(queryExpression, parameters);
        }

        public Task<IList> ListAsync(IQueryExpression queryExpression, QueryParameters parameters)
        {
            return _session.ListAsync(queryExpression, parameters);
        }

        public IQuery CreateQuery(IQueryExpression queryExpression)
        {
            return _session.CreateQuery(queryExpression);
        }

        public void List(string query, QueryParameters parameters, IList results)
        {
            _session.List(query, parameters, results);
        }

        public void List(IQueryExpression queryExpression, QueryParameters queryParameters, IList results)
        {
            _session.List(queryExpression, queryParameters, results);
        }

        public Task ListAsync(IQueryExpression queryExpression, QueryParameters queryParameters, IList results)
        {
            return _session.ListAsync(queryExpression, queryParameters, results);
        }

        public IList<T> List<T>(string query, QueryParameters queryParameters)
        {
            return _session.List<T>(query, queryParameters);
        }

        public IList<T> List<T>(IQueryExpression queryExpression, QueryParameters queryParameters)
        {
            return _session.List<T>(queryExpression, queryParameters);
        }

        public Task<IList<T>> ListAsync<T>(IQueryExpression queryExpression, QueryParameters queryParameters)
        {
            return _session.ListAsync<T>(queryExpression, queryParameters);
        }

        public IList<T> List<T>(CriteriaImpl criteria)
        {
            return _session.List<T>(criteria);
        }

        public Task<IList<T>> ListAsync<T>(CriteriaImpl criteria)
        {
            return _session.ListAsync<T>(criteria);
        }

        public void List(CriteriaImpl criteria, IList results)
        {
            _session.List(criteria, results);
        }

        public Task ListAsync(CriteriaImpl criteria, IList results)
        {
            return _session.ListAsync(criteria, results);
        }

        public IList List(CriteriaImpl criteria)
        {
            return _session.List(criteria);
        }

        public Task<IList> ListAsync(CriteriaImpl criteria)
        {
            return _session.ListAsync(criteria);
        }

        public IEnumerable Enumerable(string query, QueryParameters parameters)
        {
            return _session.Enumerable(query, parameters);
        }

        public IEnumerable Enumerable(IQueryExpression query, QueryParameters parameters)
        {
            return _session.Enumerable(query, parameters);
        }

        public Task<IEnumerable> EnumerableAsync(IQueryExpression query, QueryParameters parameters)
        {
            return _session.EnumerableAsync(query, parameters);
        }

        public IEnumerable<T> Enumerable<T>(string query, QueryParameters queryParameters)
        {
            return _session.Enumerable<T>(query, queryParameters);
        }

        public IEnumerable<T> Enumerable<T>(IQueryExpression query, QueryParameters queryParameters)
        {
            return _session.Enumerable<T>(query, queryParameters);
        }

        public Task<IEnumerable<T>> EnumerableAsync<T>(IQueryExpression query, QueryParameters queryParameters)
        {
            return _session.EnumerableAsync<T>(query, queryParameters);
        }

        public IList ListFilter(object collection, string filter, QueryParameters parameters)
        {
            return _session.ListFilter(collection, filter, parameters);
        }

        public Task<IList> ListFilterAsync(object collection, string filter, QueryParameters parameters)
        {
            return _session.ListFilterAsync(collection, filter, parameters);
        }

        public IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters)
        {
            return _session.ListFilter<T>(collection, filter, parameters);
        }

        public Task<IList<T>> ListFilterAsync<T>(object collection, string filter, QueryParameters parameters)
        {
            return _session.ListFilterAsync<T>(collection, filter, parameters);
        }

        public IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
        {
            return _session.EnumerableFilter(collection, filter, parameters);
        }

        public Task<IEnumerable> EnumerableFilterAsync(object collection, string filter, QueryParameters parameters)
        {
            return _session.EnumerableFilterAsync(collection, filter, parameters);
        }

        public IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
        {
            return _session.EnumerableFilter<T>(collection, filter, parameters);
        }

        public Task<IEnumerable<T>> EnumerableFilterAsync<T>(object collection, string filter, QueryParameters parameters)
        {
            return _session.EnumerableFilterAsync<T>(collection, filter, parameters);
        }

        public IEntityPersister GetEntityPersister(string entityName, object obj)
        {
            return _session.GetEntityPersister(entityName, obj);
        }

        public void AfterTransactionBegin(ITransaction tx)
        {
            _session.AfterTransactionBegin(tx);
        }

        public Task BeforeTransactionCompletion(ITransaction tx)
        {
            return _session.BeforeTransactionCompletion(tx);
        }

        public Task AfterTransactionCompletion(bool successful, ITransaction tx)
        {
            return _session.AfterTransactionCompletion(successful, tx);
        }

        public object GetContextEntityIdentifier(object obj)
        {
            return _session.GetContextEntityIdentifier(obj);
        }

        public object Instantiate(string entityName, object id)
        {
            return _session.Instantiate(entityName, id);
        }

        public IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
        {
            return _session.List(spec, queryParameters);
        }

        public Task<IList> ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
        {
            return _session.ListAsync(spec, queryParameters);
        }

        public void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
        {
            _session.List(spec, queryParameters, results);
        }

        public Task ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
        {
            return _session.ListAsync(spec, queryParameters, results);
        }

        public IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
        {
            return _session.List<T>(spec, queryParameters);
        }

        public Task<IList<T>> ListAsync<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
        {
            return _session.ListAsync<T>(spec, queryParameters);
        }

        public void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
        {
            _session.ListCustomQuery(customQuery, queryParameters, results);
        }

        public Task ListCustomQueryAsync(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
        {
            return _session.ListCustomQueryAsync(customQuery, queryParameters, results);
        }

        public IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
        {
            return _session.ListCustomQuery<T>(customQuery, queryParameters);
        }

        public Task<IList<T>> ListCustomQueryAsync<T>(ICustomQuery customQuery, QueryParameters queryParameters)
        {
            return _session.ListCustomQueryAsync<T>(customQuery, queryParameters);
        }

        public object GetFilterParameterValue(string filterParameterName)
        {
            return _session.GetFilterParameterValue(filterParameterName);
        }

        public IType GetFilterParameterType(string filterParameterName)
        {
            return _session.GetFilterParameterType(filterParameterName);
        }

        public IQuery GetNamedSQLQuery(string name)
        {
            return _session.GetNamedSQLQuery(name);
        }

        public IQueryTranslator[] GetQueries(string query, bool scalar)
        {
            return _session.GetQueries(query, scalar);
        }

        public Task<IQueryTranslator[]> GetQueries(IQueryExpression query, bool scalar)
        {
            return _session.GetQueries(query, scalar);
        }

        public object GetEntityUsingInterceptor(EntityKey key)
        {
            return _session.GetEntityUsingInterceptor(key);
        }

        public string BestGuessEntityName(object entity)
        {
            return _session.BestGuessEntityName(entity);
        }

        public string GuessEntityName(object entity)
        {
            return _session.GuessEntityName(entity);
        }

        public Task<DbConnection> GetConnection()
        {
            return _session.GetConnection();
        }

        public Task<IQuery> CreateFilterAsync(object collection, string queryString)
        {
            return _session.CreateFilterAsync(collection, queryString);
        }

        IQuery ISession.GetNamedQuery(string queryName)
        {
            return ((ISession) _session).GetNamedQuery(queryName);
        }

        public ISQLQuery CreateSQLQuery(string queryString)
        {
            return _session.CreateSQLQuery(queryString);
        }

        public void Clear()
        {
            _session.Clear();
        }

        public object Get(Type clazz, object id)
        {
            return _session.Get(clazz, id);
        }

        public Task<object> GetAsync(Type clazz, object id)
        {
            return _session.GetAsync(clazz, id);
        }

        public object Get(Type clazz, object id, LockMode lockMode)
        {
            return _session.Get(clazz, id, lockMode);
        }

        public Task<object> GetAsync(Type clazz, object id, LockMode lockMode)
        {
            return _session.GetAsync(clazz, id, lockMode);
        }

        public object Get(string entityName, object id)
        {
            return _session.Get(entityName, id);
        }

        public Task<object> GetAsync(string entityName, object id)
        {
            return _session.GetAsync(entityName, id);
        }

        public T Get<T>(object id)
        {
            return _session.Get<T>(id);
        }

        public Task<T> GetAsync<T>(object id)
        {
            return _session.GetAsync<T>(id);
        }

        public T Get<T>(object id, LockMode lockMode)
        {
            return _session.Get<T>(id, lockMode);
        }

        public Task<T> GetAsync<T>(object id, LockMode lockMode)
        {
            return _session.GetAsync<T>(id, lockMode);
        }

        public string GetEntityName(object obj)
        {
            return _session.GetEntityName(obj);
        }

        public IFilter EnableFilter(string filterName)
        {
            return _session.EnableFilter(filterName);
        }

        public IFilter GetEnabledFilter(string filterName)
        {
            return _session.GetEnabledFilter(filterName);
        }

        public void DisableFilter(string filterName)
        {
            _session.DisableFilter(filterName);
        }

        public IMultiQuery CreateMultiQuery()
        {
            return _session.CreateMultiQuery();
        }

        public ISession SetBatchSize(int batchSize)
        {
            return _session.SetBatchSize(batchSize);
        }

        public ISessionImplementor GetSessionImplementation()
        {
            return this;
        }

        public IMultiCriteria CreateMultiCriteria()
        {
            return _session.CreateMultiCriteria();
        }

        public ISession GetSession(EntityMode entityMode)
        {
            return _session.GetSession(entityMode);
        }

        public EntityMode ActiveEntityMode
        {
            get { return _session.ActiveEntityMode; }
        }

        FlushMode ISession.FlushMode
        {
            get { return ((ISession)_session).FlushMode; }
            set { ((ISession)_session).FlushMode = value; }
        }

        CacheMode ISession.CacheMode
        {
            get { return ((ISession)_session).CacheMode; }
            set { ((ISession)_session).CacheMode = value; }
        }

        IQuery ISessionImplementor.GetNamedQuery(string queryName)
        {
            return ((ISessionImplementor) _session).GetNamedQuery(queryName);
        }

        void ISessionImplementor.Flush()
        {
            _eventAggregator.SendMessage(new SessionFlushingEvent(this));
            ((ISessionImplementor) _session).Flush();
        }

        async Task ISession.FlushAsync()
        {
            await _eventAggregator.SendMessageAsync(new SessionFlushingEvent(this));
            await ((ISession) _session).FlushAsync();
        }

        public IDbConnection Disconnect()
        {
            return _session.Disconnect();
        }

        public void Reconnect()
        {
            _session.Reconnect();
        }

        public void Reconnect(IDbConnection connection)
        {
            _session.Reconnect(connection);
        }

        public IDbConnection Close()
        {
            return _session.Close();
        }

        public void CancelQuery()
        {
            _session.CancelQuery();
        }

        public bool IsDirty()
        {
            return _session.IsDirty();
        }

        public Task<bool> IsDirtyAsync()
        {
            return _session.IsDirtyAsync();
        }

        public bool IsReadOnly(object entityOrProxy)
        {
            return _session.IsReadOnly(entityOrProxy);
        }

        public void SetReadOnly(object entityOrProxy, bool readOnly)
        {
            _session.SetReadOnly(entityOrProxy, readOnly);
        }

        public object GetIdentifier(object obj)
        {
            return _session.GetIdentifier(obj);
        }

        public bool Contains(object obj)
        {
            return _session.Contains(obj);
        }

        public void Evict(object obj)
        {
            _session.Evict(obj);
        }

        public Task EvictAsync(object obj)
        {
            return _session.EvictAsync(obj);
        }

        public object Load(Type theType, object id, LockMode lockMode)
        {
            return _session.Load(theType, id, lockMode);
        }

        public Task<object> LoadAsync(Type theType, object id, LockMode lockMode)
        {
            return _session.LoadAsync(theType, id, lockMode);
        }

        public object Load(string entityName, object id, LockMode lockMode)
        {
            return _session.Load(entityName, id, lockMode);
        }

        public Task<object> LoadAsync(string entityName, object id, LockMode lockMode)
        {
            return _session.LoadAsync(entityName, id, lockMode);
        }

        public object Load(Type theType, object id)
        {
            return _session.Load(theType, id);
        }

        public Task<object> LoadAsync(Type theType, object id)
        {
            return _session.LoadAsync(theType, id);
        }

        public T Load<T>(object id, LockMode lockMode)
        {
            return _session.Load<T>(id, lockMode);
        }

        public Task<T> LoadAsync<T>(object id, LockMode lockMode)
        {
            return _session.LoadAsync<T>(id, lockMode);
        }

        public T Load<T>(object id)
        {
            return _session.Load<T>(id);
        }

        public Task<T> LoadAsync<T>(object id)
        {
            return _session.LoadAsync<T>(id);
        }

        public object Load(string entityName, object id)
        {
            return _session.Load(entityName, id);
        }

        public Task<object> LoadAsync(string entityName, object id)
        {
            return _session.LoadAsync(entityName, id);
        }

        public void Load(object obj, object id)
        {
            _session.Load(obj, id);
        }

        public Task LoadAsync(object obj, object id)
        {
            return _session.LoadAsync(obj, id);
        }

        public void Replicate(object obj, ReplicationMode replicationMode)
        {
            _session.Replicate(obj, replicationMode);
        }

        public Task ReplicateAsync(object obj, ReplicationMode replicationMode)
        {
            return _session.ReplicateAsync(obj, replicationMode);
        }

        public void Replicate(string entityName, object obj, ReplicationMode replicationMode)
        {
            _session.Replicate(entityName, obj, replicationMode);
        }

        public Task ReplicateAsync(string entityName, object obj, ReplicationMode replicationMode)
        {
            return _session.ReplicateAsync(entityName, obj, replicationMode);
        }

        public object Save(object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            return _session.Save(obj);
        }

        public async Task<object> SaveAsync(object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            return await _session.SaveAsync(obj);
        }

        public void Save(object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            _session.Save(obj, id);
        }

        public async Task SaveAsync(object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await _session.SaveAsync(obj, id);
        }

        public object Save(string entityName, object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            return _session.Save(entityName, obj);
        }

        public async Task<object> SaveAsync(string entityName, object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            return await _session.SaveAsync(entityName, obj);
        }

        public void Save(string entityName, object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            _session.Save(entityName, obj, id);
        }

        public async Task SaveAsync(string entityName, object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await _session.SaveAsync(entityName, obj, id);
        }

        public void SaveOrUpdate(object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            _session.SaveOrUpdate(obj);
        }

        public async Task SaveOrUpdateAsync(object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await _session.SaveOrUpdateAsync(obj);
        }

        public void SaveOrUpdate(string entityName, object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            _session.SaveOrUpdate(entityName, obj);
        }

        public async Task SaveOrUpdateAsync(string entityName, object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await _session.SaveOrUpdateAsync(entityName, obj);
        }

        public void SaveOrUpdate(string entityName, object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            _session.SaveOrUpdate(entityName, obj, id);
        }

        public async Task SaveOrUpdateAsync(string entityName, object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await _session.SaveOrUpdateAsync(entityName, obj, id);
        }

        public void Update(object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            _session.Update(obj);
        }

        public async Task UpdateAsync(object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await _session.UpdateAsync(obj);
        }

        public void Update(object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            _session.Update(obj, id);
        }

        public async Task UpdateAsync(object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await _session.UpdateAsync(obj, id);
        }

        public void Update(string entityName, object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            _session.Update(entityName, obj);
        }

        public async Task UpdateAsync(string entityName, object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await _session.UpdateAsync(entityName, obj);
        }

        public void Update(string entityName, object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            _session.Update(entityName, obj, id);
        }

        public async Task UpdateAsync(string entityName, object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await _session.UpdateAsync(entityName, obj, id);
        }

        public object Merge(object obj)
        {
            return _session.Merge(obj);
        }

        public Task<object> MergeAsync(object obj)
        {
            return _session.MergeAsync(obj);
        }

        public object Merge(string entityName, object obj)
        {
            return _session.Merge(entityName, obj);
        }

        public Task<object> MergeAsync(string entityName, object obj)
        {
            return _session.MergeAsync(entityName, obj);
        }

        public T Merge<T>(T entity) where T : class
        {
            return _session.Merge(entity);
        }

        public Task<T> MergeAsync<T>(T entity) where T : class
        {
            return _session.MergeAsync(entity);
        }

        public T Merge<T>(string entityName, T entity) where T : class
        {
            return _session.Merge(entityName, entity);
        }

        public Task<T> MergeAsync<T>(string entityName, T entity) where T : class
        {
            return _session.MergeAsync(entityName, entity);
        }

        public void Persist(object obj)
        {
            _session.Persist(obj);
        }

        public Task PersistAsync(object obj)
        {
            return _session.PersistAsync(obj);
        }

        public void Persist(string entityName, object obj)
        {
            _session.Persist(entityName, obj);
        }

        public Task PersistAsync(string entityName, object obj)
        {
            return _session.PersistAsync(entityName, obj);
        }

        public void Delete(object obj)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            _session.Delete(obj);
        }

        public async Task DeleteAsync(object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            await _session.DeleteAsync(obj);
        }

        public void Delete(string entityName, object obj)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            _session.Delete(entityName, obj);
        }

        public async Task DeleteAsync(string entityName, object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            await _session.DeleteAsync(entityName, obj);
        }

        public int Delete(string query)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            return _session.Delete(query);
        }

        public async Task<int> DeleteAsync(string query)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            return await _session.DeleteAsync(query);
        }

        public int Delete(string query, object value, IType type)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            return _session.Delete(query, value, type);
        }

        public async Task<int> DeleteAsync(string query, object value, IType type)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            return await _session.DeleteAsync(query, value, type);
        }

        public int Delete(string query, object[] values, IType[] types)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            return _session.Delete(query, values, types);
        }

        public async Task<int> DeleteAsync(string query, object[] values, IType[] types)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            return await _session.DeleteAsync(query, values, types);
        }

        public void Lock(object obj, LockMode lockMode)
        {
            _session.Lock(obj, lockMode);
        }

        public Task LockAsync(object obj, LockMode lockMode)
        {
            return _session.LockAsync(obj, lockMode);
        }

        public void Lock(string entityName, object obj, LockMode lockMode)
        {
            _session.Lock(entityName, obj, lockMode);
        }

        public Task LockAsync(string entityName, object obj, LockMode lockMode)
        {
            return _session.LockAsync(entityName, obj, lockMode);
        }

        public void Refresh(object obj)
        {
            _session.Refresh(obj);
        }

        public Task RefreshAsync(object obj)
        {
            return _session.RefreshAsync(obj);
        }

        public void Refresh(object obj, LockMode lockMode)
        {
            _session.Refresh(obj, lockMode);
        }

        public Task RefreshAsync(object obj, LockMode lockMode)
        {
            return _session.RefreshAsync(obj, lockMode);
        }

        public LockMode GetCurrentLockMode(object obj)
        {
            return _session.GetCurrentLockMode(obj);
        }

        public ITransaction BeginTransaction()
        {
            return _session.BeginTransaction();
        }

        public Task<ITransaction> BeginTransactionAsync()
        {
            return _session.BeginTransactionAsync();
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return _session.BeginTransaction(isolationLevel);
        }

        public Task<ITransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            return _session.BeginTransactionAsync(isolationLevel);
        }

        public ICriteria CreateCriteria<T>() where T : class
        {
            return _session.CreateCriteria<T>();
        }

        public ICriteria CreateCriteria<T>(string alias) where T : class
        {
            return _session.CreateCriteria<T>(alias);
        }

        public ICriteria CreateCriteria(Type persistentClass)
        {
            return _session.CreateCriteria(persistentClass);
        }

        public ICriteria CreateCriteria(Type persistentClass, string alias)
        {
            return _session.CreateCriteria(persistentClass, alias);
        }

        public ICriteria CreateCriteria(string entityName)
        {
            return _session.CreateCriteria(entityName);
        }

        public ICriteria CreateCriteria(string entityName, string alias)
        {
            return _session.CreateCriteria(entityName, alias);
        }

        public IQueryOver<T, T> QueryOver<T>() where T : class
        {
            return _session.QueryOver<T>();
        }

        public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class
        {
            return _session.QueryOver(alias);
        }

        public IQueryOver<T, T> QueryOver<T>(string entityName) where T : class
        {
            return _session.QueryOver<T>(entityName);
        }

        public IQueryOver<T, T> QueryOver<T>(string entityName, Expression<Func<T>> alias) where T : class
        {
            return _session.QueryOver(entityName, alias);
        }

        public IQuery CreateQuery(string queryString)
        {
            return _session.CreateQuery(queryString);
        }

        public IQuery CreateFilter(object collection, string queryString)
        {
            return _session.CreateFilter(collection, queryString);
        }

        void ISession.Flush()
        {
            _eventAggregator.SendMessage(new SessionFlushingEvent(this));
            ((ISession) _session).Flush();
        }

        async Task ISessionImplementor.FlushAsync()
        {
            await _eventAggregator.SendMessageAsync(new SessionFlushingEvent(this));
            await ((ISessionImplementor) _session).FlushAsync();
        }

        public int ExecuteNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters)
        {
            return _session.ExecuteNativeUpdate(specification, queryParameters);
        }

        public Task<int> ExecuteNativeUpdateAsync(NativeSQLQuerySpecification specification, QueryParameters queryParameters)
        {
            return _session.ExecuteNativeUpdateAsync(specification, queryParameters);
        }

        public int ExecuteUpdate(string query, QueryParameters queryParameters)
        {
            return _session.ExecuteUpdate(query, queryParameters);
        }

        public int ExecuteUpdate(IQueryExpression query, QueryParameters queryParameters)
        {
            return _session.ExecuteUpdate(query, queryParameters);
        }

        public Task<int> ExecuteUpdateAsync(IQueryExpression query, QueryParameters queryParameters)
        {
            return _session.ExecuteUpdateAsync(query, queryParameters);
        }

        public void CloseSessionFromDistributedTransaction()
        {
            _session.CloseSessionFromDistributedTransaction();
        }

        public EntityKey GenerateEntityKey(object id, IEntityPersister persister)
        {
            return _session.GenerateEntityKey(id, persister);
        }

        public CacheKey GenerateCacheKey(object id, IType type, string entityOrRoleName)
        {
            return _session.GenerateCacheKey(id, type, entityOrRoleName);
        }

        public long Timestamp
        {
            get { return _session.Timestamp; }
        }

        public ISessionFactoryImplementor Factory
        {
            get { return _session.Factory; }
        }

        public IBatcher Batcher
        {
            get { return _session.Batcher; }
        }

        public IDictionary<string, IFilter> EnabledFilters
        {
            get { return _session.EnabledFilters; }
        }

        public IInterceptor Interceptor
        {
            get { return _session.Interceptor; }
        }

        public NHibernate.Event.EventListeners Listeners
        {
            get { return _session.Listeners; }
        }

        public int DontFlushFromFind
        {
            get { return _session.DontFlushFromFind; }
        }

        public ConnectionManager ConnectionManager
        {
            get { return _session.ConnectionManager; }
        }

        public bool IsEventSource
        {
            get { return _session.IsEventSource; }
        }

        public IPersistenceContext PersistenceContext
        {
            get { return _session.PersistenceContext; }
        }

        CacheMode ISessionImplementor.CacheMode
        {
            get { return ((ISessionImplementor)_session).CacheMode; }
            set { ((ISessionImplementor)_session).CacheMode = value; }
        }

        IDbConnection ISession.Connection
        {
            get { return ((ISession)_session).Connection; }
        }

        bool ISession.IsOpen
        {
            get { return ((ISession)_session).IsOpen; }
        }

        bool ISession.IsConnected
        {
            get { return ((ISession)_session).IsConnected; }
        }

        public bool DefaultReadOnly
        {
            get { return _session.DefaultReadOnly; }
            set { _session.DefaultReadOnly = value; }
        }

        public ITransaction Transaction
        {
            get { return _session.Transaction; }
        }

        public ISessionStatistics Statistics
        {
            get { return _session.Statistics; }
        }

        bool ISessionImplementor.IsOpen
        {
            get { return ((ISessionImplementor)_session).IsOpen; }
        }

        bool ISessionImplementor.IsConnected
        {
            get { return ((ISessionImplementor)_session).IsConnected; }
        }

        FlushMode ISessionImplementor.FlushMode
        {
            get { return ((ISessionImplementor)_session).FlushMode; }
            set { ((ISessionImplementor)_session).FlushMode = value; }
        }

        public string FetchProfile
        {
            get { return _session.FetchProfile; }
            set { _session.FetchProfile = value; }
        }

        public ISessionFactory SessionFactory
        {
            get { return _session.SessionFactory; }
        }

        IDbConnection ISessionImplementor.Connection
        {
            get { return ((ISessionImplementor)_session).Connection; }
        }

        public bool IsClosed
        {
            get { return _session.IsClosed; }
        }

        public bool TransactionInProgress
        {
            get { return _session.TransactionInProgress; }
        }

        public EntityMode EntityMode
        {
            get { return _session.EntityMode; }
        }

        public FutureCriteriaBatch FutureCriteriaBatch
        {
            get { return _session.FutureCriteriaBatch; }
        }

        public FutureQueryBatch FutureQueryBatch
        {
            get { return _session.FutureQueryBatch; }
        }

        public Guid SessionId
        {
            get { return _session.SessionId; }
        }

        public ITransactionContext TransactionContext
        {
            get { return _session.TransactionContext; }
            set { _session.TransactionContext = value; }
        }

        public object CustomContext
        {
            get { return _session.CustomContext; }
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        public object Instantiate(IEntityPersister persister, object id)
        {
            return _session.Instantiate(persister, id);
        }

        public async Task ForceFlush(EntityEntry e)
        {
            await _eventAggregator.SendMessageAsync(new SessionFlushingEvent(this));
            await _session.ForceFlush(e);
        }

        public Task Merge(string entityName, object obj, IDictionary copiedAlready)
        {
            return _session.Merge(entityName, obj, copiedAlready);
        }

        public Task Persist(string entityName, object obj, IDictionary createdAlready)
        {
            return _session.Persist(entityName, obj, createdAlready);
        }

        public Task PersistOnFlush(string entityName, object obj, IDictionary copiedAlready)
        {
            return _session.PersistOnFlush(entityName, obj, copiedAlready);
        }

        public Task Refresh(object obj, IDictionary refreshedAlready)
        {
            return _session.Refresh(obj, refreshedAlready);
        }

        public async Task Delete(string entityName, object child, bool isCascadeDeleteEnabled, ISet<object> transientEntities)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            await _session.Delete(entityName, child, isCascadeDeleteEnabled, transientEntities);
        }

        public ActionQueue ActionQueue
        {
            get { return _session.ActionQueue; }
        }
    }
}
