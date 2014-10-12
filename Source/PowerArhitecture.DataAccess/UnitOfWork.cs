using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using PowerArhitecture.DataAccess.Managers;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Wrappers;
using PowerArhitecture.Domain;
using NHibernate;
using NHibernate.Linq;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Extensions.NamedScope;
using Ninject.Syntax;
using LockMode = PowerArhitecture.DataAccess.Enums.LockMode;

namespace PowerArhitecture.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IUnitOfWorkResolution
    {
        private readonly ILogger _logger;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IResolutionRoot _resolutionRoot;
        private readonly ISession _session;

        public UnitOfWork(ILogger logger, IRepositoryFactory repositoryFactory, IResolutionRoot resolutionRoot, ISession session, 
            IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _logger = logger;
            session.BeginTransaction(isolationLevel);
            //SessionManager.BeginTransaction(session, isolationLevel);
            /*
            var sessFactorywrapper = sessionFactory as SessionFactoryWrapper;
            if (sessFactorywrapper != null)
                sessionFactory = sessFactorywrapper.SessionFactory; //We want to skip the automatic handling of session
            Session = sessionFactory.OpenSession();
            Session.BeginTransaction(isolationLevel);

            var childKernel = new ChildKernel(resolutionRoot);
            childKernel.Bind<ISession>().ToConstant(Session).DefinesNamedScope("Test");
            childKernel.Bind<IKernel, IResolutionRoot>().ToConstant(childKernel);
            _repositoryFactory = childKernel.Get<IRepositoryFactory>();*/
            _repositoryFactory = repositoryFactory;
            _resolutionRoot = resolutionRoot;
            _session = session;
        }

        public IQueryable<TModel> Query<TModel>() where TModel : class, IEntity<long>, new()
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

        public void Save<TModel>(TModel model) where TModel : class, IEntity, new()
        {
            _session.SaveOrUpdate(model);
        }

        public void Save(params object[] models)
        {
            SaveInternal(models);
        }

        public TModel Merge<TModel>(TModel model) where TModel : class, IEntity, new()
        {
            return _session.Merge(model);
        }

        public TModel DeepCopy<TModel>(TModel model) where TModel : class, IEntity, new()
        {
            return _session.DeepCopy(model);
        }

        public IEnumerable<TModel> DeepCopy<TModel>(IEnumerable<TModel> list) where TModel : class, IEntity, new()
        {
            return _session.DeepCopy(list);
        }

        public TModel Get<TModel, TId>(TId id) where TModel : class, IEntity<TId>, new()
        {
            return _session.Get<TModel>(id);
        }

        public TModel Get<TModel>(long id) where TModel : class, IEntity<long>, new()
        {
            return _session.Get<TModel>(id);
        }

        /// <summary>
        /// Should not be used - only for test purposes
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        public void Refresh<TModel>(TModel model) where TModel : class, IEntity, new()
        {
            _session.Refresh(model);
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

        public T Get<T>()
        {
            return _resolutionRoot.Get<T>();
        }
    }
}
