using System;
using PowerArhitecture.Domain;
using NHibernate;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IRepositoryFactory
    {
        IRepository<TModel, long> GetRepository<TModel>() where TModel : class, IEntity<long>, new();

        IRepository GetRepository(Type modelType, Type idType);

        IRepository<TModel, TId> GetRepository<TModel, TId>() where TModel : class, IEntity<TId>, new();

        IRepository GetRepository(ISession session, Type modelType, Type idType);

        IRepository<TModel, TId> GetRepository<TModel, TId>(ISession session) where TModel : class, IEntity<TId>, new();

        TRepo GetCustomRepository<TRepo>() where TRepo : IRepository;

        TRepo GetCustomRepository<TRepo>(ISession session) where TRepo : IRepository;
    }
}
