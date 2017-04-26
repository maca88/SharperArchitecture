using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using NHibernate;
using NHibernate.Linq;
using SharperArchitecture.Common.Exceptions;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Events;
using SharperArchitecture.DataAccess.Extensions;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using IsolationLevel = System.Data.IsolationLevel;

namespace SharperArchitecture.DataAccess
{
    internal class UnitOfWork : IUnitOfWorkImplementor
    {
        private readonly Scope _scope;
        private readonly TransactionScope _transactionScope;
        private readonly Container _container;
        private readonly IEventSubscriber _eventSubscriber;
        private readonly ConcurrentDictionary<string, ISession> _sessions = new ConcurrentDictionary<string, ISession>();
        private readonly string _guid = Guid.NewGuid().ToString();

        public UnitOfWork(Container container, IEventSubscriber eventSubscriber, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _container = container;
            IsolationLevel = isolationLevel;
            _scope = _container.BeginExecutionContextScope();
            _scope.SetItem(ScopeKey, _guid);
            _eventSubscriber = eventSubscriber;
            _eventSubscriber.Subscribe<SessionCreatedEvent>(OnSessionCreated);
            if (Database.MultipleDatabases)
            {
                _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            }
        }

        internal const string ScopeKey = "UnitOfWorkScope";

        public IsolationLevel IsolationLevel { get; }

        public IQueryable<TModel> Query<TModel>() where TModel : IEntity
        {
            return GetOrAddSession<TModel>().Query<TModel>();
        }

        public T Load<T>(object id) where T : IEntity
        {
            return GetOrAddSession<T>().Load<T>(id);
        }

        public async Task<T> LoadAsync<T>(object id) where T : IEntity
        {
            return await GetOrAddSession<T>().LoadAsync<T>(id);
        }

        public void Save(IEntity model)
        {
            GetOrAddSession(model.GetTypeUnproxied()).SaveOrUpdate(model);
        }

        public async Task SaveAsync(IEntity model)
        {
            await GetOrAddSession(model.GetTypeUnproxied()).SaveOrUpdateAsync(model);
        }

        public void Delete(IEntity model)
        {
            GetOrAddSession(model.GetTypeUnproxied()).Delete(model);
        }

        public async Task DeleteAsync(IEntity model)
        {
            await GetOrAddSession(model.GetTypeUnproxied()).DeleteAsync(model);
        }

        public TModel Get<TModel>(object id) where TModel : IEntity
        {
            return GetOrAddSession<TModel>().Get<TModel>(id);
        }

        public async Task<TModel> GetAsync<TModel>(object id) where TModel : IEntity
        {
            return await GetOrAddSession<TModel>().GetAsync<TModel>(id);
        }

        public void Refresh(IEntity model)
        {
            GetOrAddSession(model.GetTypeUnproxied()).Refresh(model);
        }

        public async Task RefreshAsync(IEntity model)
        {
            await GetOrAddSession(model.GetTypeUnproxied()).RefreshAsync(model);
        }

        public void Flush()
        {
            foreach (var session in _sessions.Values)
            {
                session.Flush();
            }
        }

        public async Task FlushAsync()
        {
            foreach (var session in _sessions.Values)
            {
                await session.FlushAsync();
            }
        }

        public void Commit()
        {
            foreach (var session in _sessions.Values)
            {
                session.CommitTransaction();
            }
            _transactionScope?.Complete();
        }

        public async Task CommitAsync()
        {
            foreach (var session in _sessions.Values)
            {
                await session.CommitTransactionAsync();
            }
            _transactionScope?.Complete();
        }

        public void Rollback()
        {
            if (_transactionScope != null)
            {
                return; // Will rollback in Dispose
            }
            foreach (var session in _sessions.Values)
            {
                session.RollbackTransaction();
            }
        }

        public IEnumerable<ISession> GetActiveSessions()
        {
            return _sessions.Values.ToArray();
        }

        public IUnitOfWorkImplementor GetUnitOfWorkImplementation()
        {
            return this;
        }

        public void Dispose()
        {
            _scope?.Dispose();
            _eventSubscriber.Unsubscribe<SessionCreatedEvent>(OnSessionCreated);
            _transactionScope?.Dispose();
            _sessions.Clear();
        }

        public ISession DefaultSession => GetOrAddSession(DatabaseConfiguration.DefaultName);

        public bool ContainsDefaultSession()
        {
            return ContainsSession(DatabaseConfiguration.DefaultName);
        }

        public bool ContainsSession(string dbConfigName)
        {
            ISession session;
            return _sessions.TryGetValue(dbConfigName, out session);
        }

        private void OnSessionCreated(SessionCreatedEvent @event)
        {
            var scope = Lifestyle.Scoped.GetCurrentScope(_container);
            if (scope.GetItem(ScopeKey) as string != _guid)
            {
                return;
            }
            @event.Session.BeginTransaction(IsolationLevel);
            if (_sessions.ContainsKey(@event.DatabaseConfigurationName))
            {
                return;
            }
            _sessions.AddOrUpdate(@event.DatabaseConfigurationName,
                s => @event.Session,
                (s, session) => session);
        }

        private ISession GetOrAddSession(string dbConfigName)
        {
            if (!Database.ContainsDatabaseConfiguration(dbConfigName))
            {
                throw new SharperArchitectureException(
                    $"There isn't any DatabaseConfiguration registered with the name '{dbConfigName}'");
            }
            dbConfigName = dbConfigName ?? DatabaseConfiguration.DefaultName;
            return _sessions.GetOrAdd(dbConfigName, k =>
            {
                var session = _container.GetDatabaseService<ISession>(dbConfigName);
                return session;
            });
        }

        private ISession GetOrAddSession<T>()
        {
            return GetOrAddSession(typeof(T));
        }

        private ISession GetOrAddSession(Type modelType)
        {
            return GetOrAddSession(modelType.GetDatabaseConfigurationNameForModel());
        }
    }
}
