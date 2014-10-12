using System;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using NHibernate;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess.Factories
{
    public class RepositoryFactory: IRepositoryFactory
    {
        readonly IResolutionRoot _resolutionRoot;

        public RepositoryFactory(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public IRepository<TModel, long> GetRepository<TModel>() where TModel : class, IEntity<long>, new()
        {
            return _resolutionRoot.Get<IRepository<TModel, long>>();
        }

        public IRepository GetRepository(Type modelType, Type idType)
        {
            var repoType = typeof (IRepository<,>).MakeGenericType(new[] {modelType, idType});
            return (IRepository) _resolutionRoot.Get(repoType);
        }

        public IRepository<TModel, TId> GetRepository<TModel, TId>() where TModel : class, IEntity<TId>, new()
        {
            return _resolutionRoot.Get<IRepository<TModel, TId>>();
        }

        public IRepository GetRepository(ISession session, Type modelType, Type idType)
        {
            var repoType = typeof(IRepository<,>).MakeGenericType(new[] { modelType, idType });
            return (IRepository)_resolutionRoot.Get(repoType, new ConstructorArgument("session", session));
        }

        public IRepository<TModel, TId> GetRepository<TModel, TId>(ISession session) where TModel : class, IEntity<TId>, new()
        {
            return _resolutionRoot.Get<IRepository<TModel, TId>>(new ConstructorArgument("session", session));
        }

        public TRepo GetCustomRepository<TRepo>() where TRepo : IRepository
        {
            return _resolutionRoot.Get<TRepo>();
        }

        public TRepo GetCustomRepository<TRepo>(ISession session) where TRepo : IRepository
        {
            return _resolutionRoot.Get<TRepo>(new ConstructorArgument("session", session));
        }
    }
}
