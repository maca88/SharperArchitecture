using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ninject.Syntax;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.Domain;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IUnitOfWork : IDisposable
    {
        IQueryable<TModel> Query<TModel>() where TModel : class, IEntity<long>, new();

        IRepository<TModel, long> GetRepository<TModel>() where TModel : class, IEntity<long>, new();

        IRepository<TModel, TId> GetRepository<TModel, TId>() where TModel : class, IEntity<TId>, new();

        TRepo GetCustomRepository<TRepo>() where TRepo : IRepository;

        void Save<TModel>(TModel model) where TModel : IEntity;

        void Save(params object[] models);

        TModel Merge<TModel>(TModel model) where TModel : IEntity;

        TModel Get<TModel, TId>(TId id) where TModel : IEntity<TId>;

        Task<TModel> GetAsync<TModel, TId>(TId id) where TModel : IEntity<TId>;

        TModel Get<TModel>(long id) where TModel : IEntity<long>;

        Task<TModel> GetAsync<TModel>(long id) where TModel : IEntity<long>;

        void Refresh<TModel>(TModel model) where TModel : IEntity;

        IEnumerable<Type> FindMappedTypes(Func<Type, bool> condition);

        void Flush();

        Task FlushAsync();

        void Commit();

        Task CommitAsync();

        void Rollback();

        Task RollbackAsync();

        /// <summary>
        /// Use this if you want instantiate a class that has a Repository or Session as dependency
        /// </summary>
        IResolutionRoot ResolutionRoot { get; }

        void SetFlushMode(FlushMode mode);

        IUnitOfWorkImplementor GetUnitOfWorkImplementation();

    }
}
