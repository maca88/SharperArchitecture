using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using NHibernate;
using NHibernate.Linq;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Extensions.NamedScope;
using Ninject.Syntax;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.EventListeners;

namespace PowerArhitecture.DataAccess
{
    public class UnitOfWork : IUnitOfWorkImplementor
    {
        private readonly ILogger _logger;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ISession _session;

        public UnitOfWork(ILogger logger, IRepositoryFactory repositoryFactory, IResolutionRoot resolutionRoot, ISession session, 
            IEventPublisher eventPublisher, [Optional]IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _logger = logger;
            session.BeginTransaction(isolationLevel);
            session.Transaction.RegisterSynchronization(new TransactionEventListener(session.Unwrap(), eventPublisher));
            _repositoryFactory = repositoryFactory;
            ResolutionRoot = resolutionRoot;
            _session = session;
        }

        public IQueryable<TModel> Query<TModel>() where TModel : class, IEntity<long>, new()
        {
            return _session.Query<TModel>();
        }

        public IQueryable<TModel> Query<TModel, TId>() where TModel : class, IEntity<TId>, new()
        {
            return _session.Query<TModel>();
        }

        public IRepository<TModel, long> GetRepository<TModel>() where TModel : class, IEntity<long>, new()
        {
            return _repositoryFactory.GetRepository<TModel, long>(_session);
        }

        public IRepository<TModel, TId> GetRepository<TModel, TId>() where TModel : class, IEntity<TId>, new()
        {
            return _repositoryFactory.GetRepository<TModel, TId>(_session);
        }

        public TRepo GetCustomRepository<TRepo>()  where TRepo : IRepository
        {
            return _repositoryFactory.GetCustomRepository<TRepo>(_session);
        }

        public void Save<TModel>(TModel model) where TModel : IEntity
        {
            _session.SaveOrUpdate(model);
        }

        public Task SaveAsync<TModel>(TModel model) where TModel : IEntity
        {
            return _session.SaveOrUpdateAsync(model);
        }

        public void Save(params object[] models)
        {
            SaveInternal(models);
        }

        public Task SaveAsync(params object[] models)
        {
            return SaveInternalAsync(models);
        }

        public void Delete<TModel>(TModel model) where TModel : IEntity
        {
            _session.Delete(model);
        }

        public Task DeleteAsync<TModel>(TModel model) where TModel : IEntity
        {
            return _session.DeleteAsync(model);
        }

        public TModel Get<TModel, TId>(TId id) where TModel : IEntity<TId>
        {
            return _session.Get<TModel>(id);
        }

        public Task<TModel> GetAsync<TModel, TId>(TId id) where TModel : IEntity<TId>
        {
            return _session.GetAsync<TModel>(id);
        }

        public TModel Get<TModel>(long id) where TModel : IEntity<long>
        {
            return _session.Get<TModel>(id);
        }

        public Task<TModel> GetAsync<TModel>(long id) where TModel : IEntity<long>
        {
            return _session.GetAsync<TModel>(id);
        }

        /// <summary>
        /// Should not be used - only for test purposes
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        public void Refresh<TModel>(TModel model) where TModel : IEntity
        {
            _session.Refresh(model);
        }

        public Task RefreshAsync<TModel>(TModel model) where TModel : IEntity
        {
            return _session.RefreshAsync(model);
        }

        public IEnumerable<Type> FindMappedTypes(Func<Type, bool> condition)
        {
            return Database.GetSessionFactoryInfo(_session.SessionFactory)
                .Configuration.ClassMappings
                .Select(o => o.MappedClass)
                .Where(condition);
        }

        public IEnumerable<PropertyInfo> GetMappedProperties<TModel>()
        {
            var typeProps = typeof (TModel).GetProperties().ToDictionary(o => o.Name);
            return Database.GetSessionFactoryInfo(_session.SessionFactory)
                .Configuration.ClassMappings
                .Where(o => o.MappedClass == typeof(TModel))
                .SelectMany(o => o.PropertyIterator)
                .Where(o => typeProps.ContainsKey(o.Name))
                .Select(o => typeProps[o.Name]);
        }

        /// <summary>
        /// should not be used - only for test purposes
        /// </summary>
        public void Flush()
        {
            _session.Flush();
        }

        public Task FlushAsync()
        {
            return _session.FlushAsync();
        }

        public void Commit()
        {
            _session.CommitTransaction();
        }

        public Task CommitAsync()
        {
            return _session.CommitTransactionAsync();
        }

        public void Rollback()
        {
            _session.RollbackTransaction();
        }

        public IResolutionRoot ResolutionRoot { get; }

        public IUnitOfWorkImplementor GetUnitOfWorkImplementation()
        {
            return this;
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        private void SaveInternal<TModel>(IEnumerable<TModel> models) where TModel : new()
        {
            foreach (var model in models)
            {
                _session.SaveOrUpdate(model);
            }
        }

        private async Task SaveInternalAsync<TModel>(IEnumerable<TModel> models) where TModel : new()
        {
            foreach (var model in models)
            {
                await _session.SaveOrUpdateAsync(model);
            }
        }

        public ISession Session => _session;
    }
}
