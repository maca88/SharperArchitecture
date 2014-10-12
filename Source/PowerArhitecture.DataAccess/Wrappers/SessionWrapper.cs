using System;
using System.Data;
using System.Linq.Expressions;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Managers;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Stat;
using NHibernate.Type;

namespace PowerArhitecture.DataAccess.Wrappers
{
    public class SessionWrapper : ISession
    {
        private readonly IEventAggregator _eventAggregator;

        public SessionWrapper(ISession session, IEventAggregator eventAggragator)
        {
            Properties = new SessionProperties();
            Session = session;
            _eventAggregator = eventAggragator;
        }

        public ISession Session { get; private set; }

        public SessionProperties Properties { get; private set; }

        public void Dispose()
        {
            _eventAggregator.SendMessage(new SessionDisposingEvent(this));
            Session.Dispose();
            _eventAggregator.SendMessage(new SessionDisposedEvent(this));
        }

        public void Flush()
        {
            Session.Flush();
        }

        public IDbConnection Disconnect()
        {
            return Session.Disconnect();
        }

        public void Reconnect()
        {
            Session.Reconnect();
        }

        public void Reconnect(IDbConnection connection)
        {
            Session.Reconnect(connection);
        }

        public IDbConnection Close()
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

        public object Load(Type theType, object id, LockMode lockMode)
        {
            return Session.Load(theType, id, lockMode);
        }

        public object Load(string entityName, object id, LockMode lockMode)
        {
            return Session.Load(entityName, id, lockMode);
        }

        public object Load(Type theType, object id)
        {
            return Session.Load(theType, id);
        }

        public T Load<T>(object id, LockMode lockMode)
        {
            return Session.Load<T>(id, lockMode);
        }

        public T Load<T>(object id)
        {
            return Session.Load<T>(id);
        }

        public object Load(string entityName, object id)
        {
            return Session.Load(entityName, id);
        }

        public void Load(object obj, object id)
        {
            Session.Load(obj, id);
        }

        public void Replicate(object obj, ReplicationMode replicationMode)
        {
            Session.Load(obj, replicationMode);
        }

        public void Replicate(string entityName, object obj, ReplicationMode replicationMode)
        {
            Session.Replicate(entityName, obj, replicationMode);
        }

        public object Save(object obj)
        {
            return Session.Save(obj);
        }

        public void Save(object obj, object id)
        {
            Session.Save(obj, id);
        }

        public object Save(string entityName, object obj)
        {
            return Session.Save(entityName, obj);
        }

        public void Save(string entityName, object obj, object id)
        {
            Session.Save(entityName, obj, id);
        }

        public void SaveOrUpdate(object obj)
        {
            Session.SaveOrUpdate(obj);
        }

        public void SaveOrUpdate(string entityName, object obj)
        {
            Session.SaveOrUpdate(entityName, obj);
        }

        public void SaveOrUpdate(string entityName, object obj, object id)
        {
            Session.SaveOrUpdate(entityName, obj, id);
        }

        public void Update(object obj)
        {
            Session.Update(obj);
        }

        public void Update(object obj, object id)
        {
            Session.Update(obj, id);
        }

        public void Update(string entityName, object obj)
        {
            Session.Update(entityName, obj);
        }

        public void Update(string entityName, object obj, object id)
        {
            Session.Update(entityName, obj, id);
        }

        public object Merge(object obj)
        {
            return Session.Merge(obj);
        }

        public object Merge(string entityName, object obj)
        {
            return Session.Merge(entityName, obj);
        }

        public T Merge<T>(T entity) where T : class
        {
            return Session.Merge(entity);
        }

        public T Merge<T>(string entityName, T entity) where T : class
        {
            return Session.Merge(entityName, entity);
        }

        public void Persist(object obj)
        {
            Session.Persist(obj);
        }

        public void Persist(string entityName, object obj)
        {
            Session.Persist(entityName, obj);
        }

        public void Delete(object obj)
        {
            Session.Delete(obj);
        }

        public void Delete(string entityName, object obj)
        {
            Session.Delete(entityName, obj);
        }

        public int Delete(string query)
        {
            return Session.Delete(query);
        }

        public int Delete(string query, object value, IType type)
        {
            return Session.Delete(query, value, type);
        }

        public int Delete(string query, object[] values, IType[] types)
        {
            return Session.Delete(query, values, types);
        }

        public void Lock(object obj, LockMode lockMode)
        {
            Session.Lock(obj, lockMode);
        }

        public void Lock(string entityName, object obj, LockMode lockMode)
        {
            Session.Lock(entityName, obj, lockMode);
        }

        public void Refresh(object obj)
        {
            Session.Refresh(obj);
        }

        public void Refresh(object obj, LockMode lockMode)
        {
            Session.Refresh(obj, lockMode);
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

        public IQuery GetNamedQuery(string queryName)
        {
            return Session.GetNamedQuery(queryName);
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

        public object Get(Type clazz, object id, LockMode lockMode)
        {
            return Session.Get(clazz, id, lockMode);
        }

        public object Get(string entityName, object id)
        {
            return Session.Get(entityName, id);
        }

        public T Get<T>(object id)
        {
            return Session.Get<T>(id);
        }

        public T Get<T>(object id, LockMode lockMode)
        {
            return Session.Get<T>(id, lockMode);
        }

        public string GetEntityName(object obj)
        {
            return Session.GetEntityName(obj);
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
            return Session.GetSessionImplementation();
        }

        public IMultiCriteria CreateMultiCriteria()
        {
            return Session.CreateMultiCriteria();
        }

        public ISession GetSession(EntityMode entityMode)
        {
            return Session.GetSession(entityMode);
        }

        public EntityMode ActiveEntityMode { get { return Session.ActiveEntityMode; } }
        public FlushMode FlushMode
        {
            get { return Session.FlushMode; }
            set { Session.FlushMode = value; }
        }
        public CacheMode CacheMode
        {
            get { return Session.CacheMode; }
            set { Session.CacheMode = value; }
        }
        public ISessionFactory SessionFactory { get { return Session.SessionFactory; } }
        public IDbConnection Connection { get { return Session.Connection; } }
        public bool IsOpen { get { return Session.IsOpen; } }
        public bool IsConnected { get { return Session.IsConnected; } }
        public bool DefaultReadOnly
        {
            get { return Session.DefaultReadOnly; }
            set { Session.DefaultReadOnly = value; }
        }
        public ITransaction Transaction { get { return new TransactionWrapper(Session.Transaction, _eventAggregator, this); } }
        public ISessionStatistics Statistics { get { return Session.Statistics; } }

        public override bool Equals(object obj)
        {
            var toCompare = obj;
            var wrapper = obj as SessionWrapper;
            if (wrapper != null)
                toCompare = wrapper.Session;
            return Session.Equals(toCompare);
        }

        public override int GetHashCode()
        {
            return Session.GetHashCode();
        }
    }
}
