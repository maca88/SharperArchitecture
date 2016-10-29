using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.Domain;

namespace PowerArhitecture.DataAccess.Specifications
{
    //only needed for generic constraint
    public interface IRepository //: IDisposable
    {
        T Load<T>(object id);

        Task<T> LoadAsync<T>(object id);

        T Get<T>(object id);

        Task<T> GetAsync<T>(object id);

        void Save(object model);

        Task SaveAsync(object model);

        void AddListener(Action action, SessionListenerType listenerType);

        IQueryable<T> Query<T>() where T : IEntity;

        /// <summary>
        /// Retrive the current principal from the custom session context
        /// </summary>
        IPrincipal User { get; }
    } 

    public interface IRepository<TModel> : IRepository<TModel, long> where TModel : class, IEntity<long>, new() {}

    public interface IRepository<TModel, TId> : IRepository where TModel : class, IEntity<TId>, new()
    {
        IQueryable<TModel> Query();

        TModel Get(TId id);

        Task<TModel> GetAsync(TId id);

        TModel Load(TId id);

        Task<TModel> LoadAsync(TId id);

        void Save(TModel model);

        Task SaveAsync(TModel model);

        void Update(TModel model);

        Task UpdateAsync(TModel model);

        void Delete(TModel model);

        Task DeleteAsync(TModel model);

        void Delete(TId id);

        Task DeleteAsync(TId id);

        void AddListener(Action<IRepository<TModel, TId>> action, SessionListenerType listenerType);

        IEnumerable<PropertyInfo> GetMappedProperties();
    }
}
