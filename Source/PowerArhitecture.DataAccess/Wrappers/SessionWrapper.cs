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
        private readonly IEventAggregator _eventAggregator;

        public SessionWrapper(IEventSource session, IEventAggregator eventAggregator)
        {
            Session = session;
            _eventAggregator = eventAggregator;
        }

        internal readonly IEventSource Session;

        public void Initialize()
        {
            Session.Initialize();
        }

        public Task InitializeCollectionAsync(IPersistentCollection collection, bool writing)
        {
            return Session.InitializeCollectionAsync(collection, writing);
        }

        public void InitializeCollection(IPersistentCollection collection, bool writing)
        {
            Session.InitializeCollection(collection, writing);
        }

        public object InternalLoad(string entityName, object id, bool eager, bool isNullable)
        {
            return Session.InternalLoad(entityName, id, eager, isNullable);
        }

        public Task<object> InternalLoadAsync(string entityName, object id, bool eager, bool isNullable)
        {
            return Session.InternalLoadAsync(entityName, id, eager, isNullable);
        }

        public object ImmediateLoad(string entityName, object id)
        {
            return Session.ImmediateLoad(entityName, id);
        }

        public Task<object> ImmediateLoadAsync(string entityName, object id)
        {
            return Session.ImmediateLoadAsync(entityName, id);
        }

        public Task<IList> ListAsync(string query, QueryParameters parameters)
        {
            return Session.ListAsync(query, parameters);
        }

        public IList List(string query, QueryParameters parameters)
        {
            return Session.List(query, parameters);
        }

        public IList List(IQueryExpression queryExpression, QueryParameters parameters)
        {
            return Session.List(queryExpression, parameters);
        }

        public Task<IList> ListAsync(IQueryExpression queryExpression, QueryParameters parameters)
        {
            return Session.ListAsync(queryExpression, parameters);
        }

        public Task ListAsync(string query, QueryParameters parameters, IList results)
        {
            return Session.ListAsync(query, parameters, results);
        }

        public IQuery CreateQuery(IQueryExpression queryExpression)
        {
            return Session.CreateQuery(queryExpression);
        }

        public void List(string query, QueryParameters parameters, IList results)
        {
            Session.List(query, parameters, results);
        }

        public void List(IQueryExpression queryExpression, QueryParameters queryParameters, IList results)
        {
            Session.List(queryExpression, queryParameters, results);
        }

        public Task ListAsync(IQueryExpression queryExpression, QueryParameters queryParameters, IList results)
        {
            return Session.ListAsync(queryExpression, queryParameters, results);
        }

        public Task<IList<T>> ListAsync<T>(string query, QueryParameters queryParameters)
        {
            return Session.ListAsync<T>(query, queryParameters);
        }

        public IList<T> List<T>(string query, QueryParameters queryParameters)
        {
            return Session.List<T>(query, queryParameters);
        }

        public IList<T> List<T>(IQueryExpression queryExpression, QueryParameters queryParameters)
        {
            return Session.List<T>(queryExpression, queryParameters);
        }

        public Task<IList<T>> ListAsync<T>(IQueryExpression queryExpression, QueryParameters queryParameters)
        {
            return Session.ListAsync<T>(queryExpression, queryParameters);
        }

        public IList<T> List<T>(CriteriaImpl criteria)
        {
            return Session.List<T>(criteria);
        }

        public Task<IList<T>> ListAsync<T>(CriteriaImpl criteria)
        {
            return Session.ListAsync<T>(criteria);
        }

        public void List(CriteriaImpl criteria, IList results)
        {
            Session.List(criteria, results);
        }

        public Task ListAsync(CriteriaImpl criteria, IList results)
        {
            return Session.ListAsync(criteria, results);
        }

        public IList List(CriteriaImpl criteria)
        {
            return Session.List(criteria);
        }

        public Task<IList> ListAsync(CriteriaImpl criteria)
        {
            return Session.ListAsync(criteria);
        }

        public Task<IEnumerable> EnumerableAsync(string query, QueryParameters parameters)
        {
            return Session.EnumerableAsync(query, parameters);
        }

        public IEnumerable Enumerable(string query, QueryParameters parameters)
        {
            return Session.Enumerable(query, parameters);
        }

        public IEnumerable Enumerable(IQueryExpression query, QueryParameters parameters)
        {
            return Session.Enumerable(query, parameters);
        }

        public Task<IEnumerable> EnumerableAsync(IQueryExpression query, QueryParameters parameters)
        {
            return Session.EnumerableAsync(query, parameters);
        }

        public Task<IEnumerable<T>> EnumerableAsync<T>(string query, QueryParameters queryParameters)
        {
            return Session.EnumerableAsync<T>(query, queryParameters);
        }

        public IEnumerable<T> Enumerable<T>(string query, QueryParameters queryParameters)
        {
            return Session.Enumerable<T>(query, queryParameters);
        }

        public IEnumerable<T> Enumerable<T>(IQueryExpression query, QueryParameters queryParameters)
        {
            return Session.Enumerable<T>(query, queryParameters);
        }

        public Task<IEnumerable<T>> EnumerableAsync<T>(IQueryExpression query, QueryParameters queryParameters)
        {
            return Session.EnumerableAsync<T>(query, queryParameters);
        }

        public IList ListFilter(object collection, string filter, QueryParameters parameters)
        {
            return Session.ListFilter(collection, filter, parameters);
        }

        public Task<IList> ListFilterAsync(object collection, string filter, QueryParameters parameters)
        {
            return Session.ListFilterAsync(collection, filter, parameters);
        }

        public IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters)
        {
            return Session.ListFilter<T>(collection, filter, parameters);
        }

        public Task<IList<T>> ListFilterAsync<T>(object collection, string filter, QueryParameters parameters)
        {
            return Session.ListFilterAsync<T>(collection, filter, parameters);
        }

        public IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
        {
            return Session.EnumerableFilter(collection, filter, parameters);
        }

        public Task<IEnumerable> EnumerableFilterAsync(object collection, string filter, QueryParameters parameters)
        {
            return Session.EnumerableFilterAsync(collection, filter, parameters);
        }

        public IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
        {
            return Session.EnumerableFilter<T>(collection, filter, parameters);
        }

        public Task<IEnumerable<T>> EnumerableFilterAsync<T>(object collection, string filter,
            QueryParameters parameters)
        {
            return Session.EnumerableFilterAsync<T>(collection, filter, parameters);
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

        public Task<IList> ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
        {
            return Session.ListAsync(spec, queryParameters);
        }

        public void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
        {
            Session.List(spec, queryParameters, results);
        }

        public Task ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
        {
            return Session.ListAsync(spec, queryParameters, results);
        }

        public IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
        {
            return Session.List<T>(spec, queryParameters);
        }

        public Task<IList<T>> ListAsync<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
        {
            return Session.ListAsync<T>(spec, queryParameters);
        }

        public void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
        {
            Session.ListCustomQuery(customQuery, queryParameters, results);
        }

        public Task ListCustomQueryAsync(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
        {
            return Session.ListCustomQueryAsync(customQuery, queryParameters, results);
        }

        public IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
        {
            return Session.ListCustomQuery<T>(customQuery, queryParameters);
        }

        public Task<IList<T>> ListCustomQueryAsync<T>(ICustomQuery customQuery, QueryParameters queryParameters)
        {
            return Session.ListCustomQueryAsync<T>(customQuery, queryParameters);
        }

        public Task<IQueryTranslator[]> GetQueriesAsync(string query, bool scalar)
        {
            return Session.GetQueriesAsync(query, scalar);
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

        public IQueryTranslator[] GetQueries(string query, bool scalar)
        {
            return Session.GetQueries(query, scalar);
        }

        public IQueryTranslator[] GetQueries(IQueryExpression query, bool scalar)
        {
            return Session.GetQueries(query, scalar);
        }

        public Task<IQueryTranslator[]> GetQueriesAsync(IQueryExpression query, bool scalar)
        {
            return Session.GetQueriesAsync(query, scalar);
        }

        public Task<object> GetEntityUsingInterceptorAsync(EntityKey key)
        {
            return Session.GetEntityUsingInterceptorAsync(key);
        }

        public object GetEntityUsingInterceptor(EntityKey key)
        {
            return Session.GetEntityUsingInterceptor(key);
        }

        public string BestGuessEntityName(object entity)
        {
            return Session.BestGuessEntityName(entity);
        }

        public Task<string> BestGuessEntityNameAsync(object entity)
        {
            return Session.BestGuessEntityNameAsync(entity);
        }

        public string GuessEntityName(object entity)
        {
            return Session.GuessEntityName(entity);
        }

        public Task<IQuery> CreateFilterAsync(object collection, string queryString)
        {
            return Session.CreateFilterAsync(collection, queryString);
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

        public Task<object> GetAsync(Type clazz, object id)
        {
            return Session.GetAsync(clazz, id);
        }

        public object Get(Type clazz, object id, LockMode lockMode)
        {
            return Session.Get(clazz, id, lockMode);
        }

        public Task<object> GetAsync(Type clazz, object id, LockMode lockMode)
        {
            return Session.GetAsync(clazz, id, lockMode);
        }

        public object Get(string entityName, object id)
        {
            return Session.Get(entityName, id);
        }

        public Task<object> GetAsync(string entityName, object id)
        {
            return Session.GetAsync(entityName, id);
        }

        public T Get<T>(object id)
        {
            return Session.Get<T>(id);
        }

        public Task<T> GetAsync<T>(object id)
        {
            return Session.GetAsync<T>(id);
        }

        public T Get<T>(object id, LockMode lockMode)
        {
            return Session.Get<T>(id, lockMode);
        }

        public Task<T> GetAsync<T>(object id, LockMode lockMode)
        {
            return Session.GetAsync<T>(id, lockMode);
        }

        public string GetEntityName(object obj)
        {
            return Session.GetEntityName(obj);
        }

        public Task<string> GetEntityNameAsync(object obj)
        {
            return Session.GetEntityNameAsync(obj);
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
            return Session.GetSession(entityMode);
        }

        public EntityMode ActiveEntityMode => Session.ActiveEntityMode;

        FlushMode ISession.FlushMode
        {
            get { return ((ISession) Session).FlushMode; }
            set { ((ISession) Session).FlushMode = value; }
        }

        CacheMode ISession.CacheMode
        {
            get { return ((ISession) Session).CacheMode; }
            set { ((ISession) Session).CacheMode = value; }
        }

        IQuery ISessionImplementor.GetNamedQuery(string queryName)
        {
            return ((ISessionImplementor) Session).GetNamedQuery(queryName);
        }

        void ISessionImplementor.Flush()
        {
            _eventAggregator.SendMessage(new SessionFlushingEvent(this));
            ((ISessionImplementor) Session).Flush();
        }

        async Task ISession.FlushAsync()
        {
            await _eventAggregator.SendMessageAsync(new SessionFlushingEvent(this));
            await ((ISession) Session).FlushAsync();
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

        public Task<bool> IsDirtyAsync()
        {
            return Session.IsDirtyAsync();
        }

        public Task SetReadOnlyAsync(object entityOrProxy, bool readOnly)
        {
            return Session.SetReadOnlyAsync(entityOrProxy, readOnly);
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

        public Task<bool> ContainsAsync(object obj)
        {
            return Session.ContainsAsync(obj);
        }

        public void Evict(object obj)
        {
            Session.Evict(obj);
        }

        public Task EvictAsync(object obj)
        {
            return Session.EvictAsync(obj);
        }

        public object Load(Type theType, object id, LockMode lockMode)
        {
            return Session.Load(theType, id, lockMode);
        }

        public Task<object> LoadAsync(Type theType, object id, LockMode lockMode)
        {
            return Session.LoadAsync(theType, id, lockMode);
        }

        public object Load(string entityName, object id, LockMode lockMode)
        {
            return Session.Load(entityName, id, lockMode);
        }

        public Task<object> LoadAsync(string entityName, object id, LockMode lockMode)
        {
            return Session.LoadAsync(entityName, id, lockMode);
        }

        public object Load(Type theType, object id)
        {
            return Session.Load(theType, id);
        }

        public Task<object> LoadAsync(Type theType, object id)
        {
            return Session.LoadAsync(theType, id);
        }

        public T Load<T>(object id, LockMode lockMode)
        {
            return Session.Load<T>(id, lockMode);
        }

        public Task<T> LoadAsync<T>(object id, LockMode lockMode)
        {
            return Session.LoadAsync<T>(id, lockMode);
        }

        public T Load<T>(object id)
        {
            return Session.Load<T>(id);
        }

        public Task<T> LoadAsync<T>(object id)
        {
            return Session.LoadAsync<T>(id);
        }

        public object Load(string entityName, object id)
        {
            return Session.Load(entityName, id);
        }

        public Task<object> LoadAsync(string entityName, object id)
        {
            return Session.LoadAsync(entityName, id);
        }

        public void Load(object obj, object id)
        {
            Session.Load(obj, id);
        }

        public Task LoadAsync(object obj, object id)
        {
            return Session.LoadAsync(obj, id);
        }

        public void Replicate(object obj, ReplicationMode replicationMode)
        {
            Session.Replicate(obj, replicationMode);
        }

        public Task ReplicateAsync(object obj, ReplicationMode replicationMode)
        {
            return Session.ReplicateAsync(obj, replicationMode);
        }

        public void Replicate(string entityName, object obj, ReplicationMode replicationMode)
        {
            Session.Replicate(entityName, obj, replicationMode);
        }

        public Task ReplicateAsync(string entityName, object obj, ReplicationMode replicationMode)
        {
            return Session.ReplicateAsync(entityName, obj, replicationMode);
        }

        public object Save(object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            return Session.Save(obj);
        }

        public async Task<object> SaveAsync(object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            return await Session.SaveAsync(obj);
        }

        public void Save(object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            Session.Save(obj, id);
        }

        public async Task SaveAsync(object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await Session.SaveAsync(obj, id);
        }

        public object Save(string entityName, object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            return Session.Save(entityName, obj);
        }

        public async Task<object> SaveAsync(string entityName, object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            return await Session.SaveAsync(entityName, obj);
        }

        public void Save(string entityName, object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            Session.Save(entityName, obj, id);
        }

        public async Task SaveAsync(string entityName, object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await Session.SaveAsync(entityName, obj, id);
        }

        public void SaveOrUpdate(object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            Session.SaveOrUpdate(obj);
        }

        public async Task SaveOrUpdateAsync(object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await Session.SaveOrUpdateAsync(obj);
        }

        public void SaveOrUpdate(string entityName, object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            Session.SaveOrUpdate(entityName, obj);
        }

        public async Task SaveOrUpdateAsync(string entityName, object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await Session.SaveOrUpdateAsync(entityName, obj);
        }

        public void SaveOrUpdate(string entityName, object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            Session.SaveOrUpdate(entityName, obj, id);
        }

        public async Task SaveOrUpdateAsync(string entityName, object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await Session.SaveOrUpdateAsync(entityName, obj, id);
        }

        public void Update(object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            Session.Update(obj);
        }

        public async Task UpdateAsync(object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await Session.UpdateAsync(obj);
        }

        public void Update(object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            Session.Update(obj, id);
        }

        public async Task UpdateAsync(object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await Session.UpdateAsync(obj, id);
        }

        public void Update(string entityName, object obj)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            Session.Update(entityName, obj);
        }

        public async Task UpdateAsync(string entityName, object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await Session.UpdateAsync(entityName, obj);
        }

        public void Update(string entityName, object obj, object id)
        {
            _eventAggregator.SendMessage(new EntitySavingOrUpdatingEvent(this));
            Session.Update(entityName, obj, id);
        }

        public async Task UpdateAsync(string entityName, object obj, object id)
        {
            await _eventAggregator.SendMessageAsync(new EntitySavingOrUpdatingEvent(this));
            await Session.UpdateAsync(entityName, obj, id);
        }

        public object Merge(object obj)
        {
            return Session.Merge(obj);
        }

        public Task<object> MergeAsync(object obj)
        {
            return Session.MergeAsync(obj);
        }

        public object Merge(string entityName, object obj)
        {
            return Session.Merge(entityName, obj);
        }

        public Task<object> MergeAsync(string entityName, object obj)
        {
            return Session.MergeAsync(entityName, obj);
        }

        public T Merge<T>(T entity) where T : class
        {
            return Session.Merge(entity);
        }

        public Task<T> MergeAsync<T>(T entity) where T : class
        {
            return Session.MergeAsync(entity);
        }

        public T Merge<T>(string entityName, T entity) where T : class
        {
            return Session.Merge(entityName, entity);
        }

        public Task<T> MergeAsync<T>(string entityName, T entity) where T : class
        {
            return Session.MergeAsync(entityName, entity);
        }

        public void Persist(object obj)
        {
            Session.Persist(obj);
        }

        public Task PersistAsync(object obj)
        {
            return Session.PersistAsync(obj);
        }

        public void Persist(string entityName, object obj)
        {
            Session.Persist(entityName, obj);
        }

        public Task PersistAsync(string entityName, object obj)
        {
            return Session.PersistAsync(entityName, obj);
        }

        public void Delete(object obj)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            Session.Delete(obj);
        }

        public async Task DeleteAsync(object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            await Session.DeleteAsync(obj);
        }

        public void Delete(string entityName, object obj)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            Session.Delete(entityName, obj);
        }

        public async Task DeleteAsync(string entityName, object obj)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            await Session.DeleteAsync(entityName, obj);
        }

        public int Delete(string query)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            return Session.Delete(query);
        }

        public async Task<int> DeleteAsync(string query)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            return await Session.DeleteAsync(query);
        }

        public int Delete(string query, object value, IType type)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            return Session.Delete(query, value, type);
        }

        public async Task<int> DeleteAsync(string query, object value, IType type)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            return await Session.DeleteAsync(query, value, type);
        }

        public int Delete(string query, object[] values, IType[] types)
        {
            _eventAggregator.SendMessage(new EntityDeletingEvent(this));
            return Session.Delete(query, values, types);
        }

        public async Task<int> DeleteAsync(string query, object[] values, IType[] types)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            return await Session.DeleteAsync(query, values, types);
        }

        public void Lock(object obj, LockMode lockMode)
        {
            Session.Lock(obj, lockMode);
        }

        public Task LockAsync(object obj, LockMode lockMode)
        {
            return Session.LockAsync(obj, lockMode);
        }

        public void Lock(string entityName, object obj, LockMode lockMode)
        {
            Session.Lock(entityName, obj, lockMode);
        }

        public Task LockAsync(string entityName, object obj, LockMode lockMode)
        {
            return Session.LockAsync(entityName, obj, lockMode);
        }

        public void Refresh(object obj)
        {
            Session.Refresh(obj);
        }

        public Task RefreshAsync(object obj)
        {
            return Session.RefreshAsync(obj);
        }

        public void Refresh(object obj, LockMode lockMode)
        {
            Session.Refresh(obj, lockMode);
        }

        public Task RefreshAsync(object obj, LockMode lockMode)
        {
            return Session.RefreshAsync(obj, lockMode);
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
            _eventAggregator.SendMessage(new SessionFlushingEvent(this));
            ((ISession) Session).Flush();
        }

        async Task ISessionImplementor.FlushAsync()
        {
            await _eventAggregator.SendMessageAsync(new SessionFlushingEvent(this));
            await ((ISessionImplementor) Session).FlushAsync();
        }

        public int ExecuteNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters)
        {
            return Session.ExecuteNativeUpdate(specification, queryParameters);
        }

        public Task<int> ExecuteNativeUpdateAsync(NativeSQLQuerySpecification specification,
            QueryParameters queryParameters)
        {
            return Session.ExecuteNativeUpdateAsync(specification, queryParameters);
        }

        public Task<int> ExecuteUpdateAsync(string query, QueryParameters queryParameters)
        {
            return Session.ExecuteUpdateAsync(query, queryParameters);
        }

        public int ExecuteUpdate(string query, QueryParameters queryParameters)
        {
            return Session.ExecuteUpdate(query, queryParameters);
        }

        public int ExecuteUpdate(IQueryExpression query, QueryParameters queryParameters)
        {
            return Session.ExecuteUpdate(query, queryParameters);
        }

        public Task<int> ExecuteUpdateAsync(IQueryExpression query, QueryParameters queryParameters)
        {
            return Session.ExecuteUpdateAsync(query, queryParameters);
        }

        public void CloseSessionFromDistributedTransaction()
        {
            Session.CloseSessionFromDistributedTransaction();
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

        public int DontFlushFromFind => Session.DontFlushFromFind;

        public ConnectionManager ConnectionManager => Session.ConnectionManager;

        public bool IsEventSource => Session.IsEventSource;

        public IPersistenceContext PersistenceContext => Session.PersistenceContext;

        CacheMode ISessionImplementor.CacheMode
        {
            get { return ((ISessionImplementor) Session).CacheMode; }
            set { ((ISessionImplementor) Session).CacheMode = value; }
        }

        DbConnection ISession.Connection => ((ISession) Session).Connection;

        bool ISession.IsOpen => ((ISession) Session).IsOpen;

        bool ISession.IsConnected => ((ISession) Session).IsConnected;

        public bool DefaultReadOnly
        {
            get { return Session.DefaultReadOnly; }
            set { Session.DefaultReadOnly = value; }
        }

        public ITransaction Transaction => Session.Transaction;

        public ISessionStatistics Statistics => Session.Statistics;

        bool ISessionImplementor.IsOpen => ((ISessionImplementor) Session).IsOpen;

        bool ISessionImplementor.IsConnected => ((ISessionImplementor) Session).IsConnected;

        FlushMode ISessionImplementor.FlushMode
        {
            get { return ((ISessionImplementor) Session).FlushMode; }
            set { ((ISessionImplementor) Session).FlushMode = value; }
        }

        public string FetchProfile
        {
            get { return Session.FetchProfile; }
            set { Session.FetchProfile = value; }
        }

        public ISessionFactory SessionFactory => Session.SessionFactory;

        DbConnection ISessionImplementor.Connection => ((ISessionImplementor) Session).Connection;

        public bool IsClosed => Session.IsClosed;

        public bool TransactionInProgress => Session.TransactionInProgress;

        public EntityMode EntityMode => Session.EntityMode;

        public FutureCriteriaBatch FutureCriteriaBatch => Session.FutureCriteriaBatch;

        public FutureQueryBatch FutureQueryBatch => Session.FutureQueryBatch;

        public Guid SessionId => Session.SessionId;

        public ITransactionContext TransactionContext
        {
            get { return Session.TransactionContext; }
            set { Session.TransactionContext = value; }
        }

        public object UserData => Session.UserData;

        public void Dispose()
        {
            Session.Dispose();
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

        public async Task ForceFlushAsync(EntityEntry e)
        {
            await _eventAggregator.SendMessageAsync(new SessionFlushingEvent(this));
            await Session.ForceFlushAsync(e);
        }

        public Task MergeAsync(string entityName, object obj, IDictionary copiedAlready)
        {
            return Session.MergeAsync(entityName, obj, copiedAlready);
        }

        public Task PersistAsync(string entityName, object obj, IDictionary createdAlready)
        {
            return Session.PersistAsync(entityName, obj, createdAlready);
        }

        public Task PersistOnFlushAsync(string entityName, object obj, IDictionary copiedAlready)
        {
            return Session.PersistOnFlushAsync(entityName, obj, copiedAlready);
        }

        public Task RefreshAsync(object obj, IDictionary refreshedAlready)
        {
            return Session.RefreshAsync(obj, refreshedAlready);
        }

        public async Task DeleteAsync(string entityName, object child, bool isCascadeDeleteEnabled,
            ISet<object> transientEntities)
        {
            await _eventAggregator.SendMessageAsync(new EntityDeletingEvent(this));
            await Session.DeleteAsync(entityName, child, isCascadeDeleteEnabled, transientEntities);
        }

        public ActionQueue ActionQueue => Session.ActionQueue;


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
