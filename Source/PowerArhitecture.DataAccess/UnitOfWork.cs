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
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Syntax;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.EventListeners;
using PowerArhitecture.DataAccess.Parameters;
using IsolationLevel = System.Data.IsolationLevel;

namespace PowerArhitecture.DataAccess
{
    public class UnitOfWork : IUnitOfWorkImplementor
    {
        private readonly ILogger _logger;
        private readonly IsolationLevel _isolationLevel;
        private readonly IEventPublisher _eventPublisher;
        private readonly TransactionScope _transactionScope;
        private readonly ConcurrentDictionary<string, ISession> _sessions = new ConcurrentDictionary<string, ISession>();

        public UnitOfWork(ILogger logger, 
            IResolutionRoot resolutionRoot,
            IEventPublisher eventPublisher, 
            [Optional]IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _logger = logger;
            _eventPublisher = eventPublisher;
            _isolationLevel = isolationLevel;
            ResolutionRoot = resolutionRoot;
            if (Database.MultipleDatabases)
            {
                _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            }
        }

        public IQueryable<TModel> Query<TModel>() where TModel : class, IEntity<long>, new()
        {
            return GetOrAddSession<TModel>().Query<TModel>();
        }

        public IQueryable<TModel> Query<TModel>(string dbConfigName) where TModel : class, IEntity<long>, new()
        {
            if (dbConfigName == null)
            {
                throw new ArgumentNullException(nameof(dbConfigName));
            }
            return GetOrAddSession(dbConfigName).Query<TModel>();
        }

        public IQueryable<TModel> Query<TModel, TId>() where TModel : class, IEntity<TId>, new()
        {
            return GetOrAddSession<TModel>().Query<TModel>();
        }

        public IQueryable<TModel> Query<TModel, TId>(string dbConfigName) where TModel : class, IEntity<TId>, new()
        {
            if (dbConfigName == null)
            {
                throw new ArgumentNullException(nameof(dbConfigName));
            }
            return GetOrAddSession(dbConfigName).Query<TModel>();
        }

        public IRepository<TModel, long> GetRepository<TModel>() where TModel : class, IEntity<long>, new()
        {
            var dbConfigName = Database.GetDatabaseConfigurationName(typeof(TModel));
            return GetRepository<TModel>(dbConfigName);
        }

        public IRepository<TModel, long> GetRepository<TModel>(string dbConfigName) where TModel : class, IEntity<long>, new()
        {
            if (dbConfigName == null)
            {
                throw new ArgumentNullException(nameof(dbConfigName));
            }
            GetOrAddSession(dbConfigName); // Get and save the session
            return ResolutionRoot.Get<IRepository<TModel, long>>(new DatabaseConfigurationParameter(dbConfigName));
        }

        public IRepository<TModel, TId> GetRepository<TModel, TId>() where TModel : class, IEntity<TId>, new()
        {
            var dbConfigName = Database.GetDatabaseConfigurationName(typeof(TModel));
            return GetRepository<TModel, TId>(dbConfigName);
        }

        public IRepository<TModel, TId> GetRepository<TModel, TId>(string dbConfigName) where TModel : class, IEntity<TId>, new()
        {
            if (dbConfigName == null)
            {
                throw new ArgumentNullException(nameof(dbConfigName));
            }
            GetOrAddSession(dbConfigName); // Get and save the session
            return ResolutionRoot.Get<IRepository<TModel, TId>>(new DatabaseConfigurationParameter(dbConfigName));
        }

        public TRepo GetCustomRepository<TRepo>()  where TRepo : IRepository
        {
            string dbConfigName;
            if (!Database.MultipleDatabases)
            {
                dbConfigName = DatabaseConfiguration.DefaultName;
            }
            else if (!typeof(TRepo).IsAssignableToGenericType(typeof(IRepository<,>)))
            {
                throw new PowerArhitectureException(
                    $"The repository of type {typeof(TRepo)} does not implement IRepository<,>");
            }
            else
            {
                var repoType = typeof(TRepo).GetGenericType(typeof(IRepository<,>));
                dbConfigName = Database.GetDatabaseConfigurationName(repoType.GetGenericArguments()[0]);
            }
            return GetCustomRepository<TRepo>(dbConfigName);
        }

        public TRepo GetCustomRepository<TRepo>(string dbConfigName) where TRepo : IRepository
        {
            if (dbConfigName == null)
            {
                throw new ArgumentNullException(nameof(dbConfigName));
            }
            return ResolutionRoot.Get<TRepo>(new DatabaseConfigurationParameter(dbConfigName));
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

        public IResolutionRoot ResolutionRoot { get; }

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
            foreach (var session in _sessions.Values)
            {
                session.Dispose();
            }
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
            dbConfigName = dbConfigName ?? DatabaseConfiguration.DefaultName;
            return _sessions.GetOrAdd(dbConfigName, k =>
            {
                var session = ResolutionRoot.Get<ISession>(new DatabaseConfigurationParameter(dbConfigName));
                session.BeginTransaction(_isolationLevel);
                session.Transaction.RegisterSynchronization(new TransactionEventListener(session.Unwrap(), _eventPublisher));
                return session;
            });
        }

        private ISession GetOrAddSession<T>()
        {
            return GetOrAddSession(typeof(T));
        }

        private ISession GetOrAddSession(Type modelType)
        {
            return GetOrAddSession(Database.GetDatabaseConfigurationName(modelType));
        }

        
    }
}
