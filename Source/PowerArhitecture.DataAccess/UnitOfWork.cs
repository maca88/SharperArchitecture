using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using NHibernate;
using NHibernate.Linq;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Extensions;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using IsolationLevel = System.Data.IsolationLevel;

namespace PowerArhitecture.DataAccess
{
    internal class UnitOfWork : IUnitOfWorkImplementor
    {
        private bool _intialized;
        private Scope _scope;
        private TransactionScope _transactionScope;
        private readonly ILogger _logger;
        private readonly Container _container;
        private readonly ConcurrentDictionary<string, ISession> _sessions = new ConcurrentDictionary<string, ISession>();

        public UnitOfWork(ILogger logger, Container container)
        {
            _logger = logger;
            _container = container;
        }

        internal const string ScopeKey = "UnitOfWorkScope";

        public IsolationLevel IsolationLevel { get; internal set; } = IsolationLevel.Unspecified;

        public IQueryable<TModel> Query<TModel>() where TModel : class, IEntity<long>, new()
        {
            return GetOrAddSession<TModel>().Query<TModel>();
        }

        public IQueryable<TModel> Query<TModel, TId>() where TModel : class, IEntity<TId>, new()
        {
            return GetOrAddSession<TModel>().Query<TModel>();
        }

        public IRepository<TModel> GetRepository<TModel>() where TModel : class, IEntity<long>, new()
        {
            GetOrAddSession<TModel>(); // Get and save the session
            return _container.GetInstance<IRepository<TModel>>();
        }

        public IRepository<TModel, TId> GetRepository<TModel, TId>() where TModel : class, IEntity<TId>, new()
        {
            GetOrAddSession<TModel>(); // Get and save the session
            return _container.GetInstance<IRepository<TModel, TId>>();
        }

        public TRepo GetCustomRepository<TRepo>() where TRepo : class, IRepository
        {
            GetOrAddSession(typeof(TRepo).GetDatabaseConfigurationNameForRepository());
            return _container.GetInstance<TRepo>();
        }

        public void Save<TModel>(TModel model) where TModel : IEntity
        {
            GetOrAddSession<TModel>().SaveOrUpdate(model);
        }

        public Task SaveAsync<TModel>(TModel model) where TModel : IEntity
        {
            return GetOrAddSession<TModel>().SaveOrUpdateAsync(model);
        }

        public void Save(params IEntity[] models)
        {
            SaveInternal(models);
        }

        public Task SaveAsync(params IEntity[] models)
        {
            return SaveInternalAsync(models);
        }

        public void Delete<TModel>(TModel model) where TModel : IEntity
        {
            GetOrAddSession<TModel>().Delete(model);
        }

        public Task DeleteAsync<TModel>(TModel model) where TModel : IEntity
        {
            return GetOrAddSession<TModel>().DeleteAsync(model);
        }

        public TModel Get<TModel, TId>(TId id) where TModel : IEntity<TId>
        {
            return GetOrAddSession<TModel>().Get<TModel>(id);
        }

        public Task<TModel> GetAsync<TModel, TId>(TId id) where TModel : IEntity<TId>
        {
            return GetOrAddSession<TModel>().GetAsync<TModel>(id);
        }

        public TModel Get<TModel>(long id) where TModel : IEntity<long>
        {
            return GetOrAddSession<TModel>().Get<TModel>(id);
        }

        public Task<TModel> GetAsync<TModel>(long id) where TModel : IEntity<long>
        {
            return GetOrAddSession<TModel>().GetAsync<TModel>(id);
        }

        /// <summary>
        /// Should not be used - only for test purposes
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        public void Refresh<TModel>(TModel model) where TModel : IEntity
        {
            GetOrAddSession<TModel>().Refresh(model);
        }

        public Task RefreshAsync<TModel>(TModel model) where TModel : IEntity
        {
            return GetOrAddSession<TModel>().RefreshAsync(model);
        }

        /// <summary>
        /// Should not be used - only for test purposes
        /// </summary>
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
            _transactionScope?.Dispose();
            _sessions.Clear();
        }

        private void SaveInternal(IEnumerable<IEntity> models)
        {
            foreach (var model in models)
            {
                GetOrAddSession(model.GetTypeUnproxied()).SaveOrUpdate(model);
            }
        }

        private async Task SaveInternalAsync(IEnumerable<IEntity> models)
        {
            foreach (var model in models)
            {
                await GetOrAddSession(model.GetTypeUnproxied()).SaveOrUpdateAsync(model);
            }
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

        private ISession GetOrAddSession(string dbConfigName)
        {
            if (!Database.ContainsDatabaseConfiguration(dbConfigName))
            {
                throw new PowerArhitectureException(
                    $"There isn't any DatabaseConfiguration registered with the name '{dbConfigName}'");
            }
            Initialize();
            dbConfigName = dbConfigName ?? DatabaseConfiguration.DefaultName;
            return _sessions.GetOrAdd(dbConfigName, k =>
            {
                var session = _container.GetDatabaseService<ISession>(dbConfigName);
                session.BeginTransaction(IsolationLevel);
                return session;
            });
        }

        /// <summary>
        /// We must lazy initialize the scope as the constructor is called in the Verify method of the SimpleInjector and never disposed
        /// </summary>
        private void Initialize()
        {
            if (_intialized)
            {
                return;
            }
            _intialized = true;
            _scope = _container.BeginExecutionContextScope();
            _scope.SetItem(ScopeKey, true);
            if (Database.MultipleDatabases)
            {
                _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            }
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
