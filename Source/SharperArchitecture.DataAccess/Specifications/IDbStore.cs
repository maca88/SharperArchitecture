using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface IDbStore
    {
        IQueryable<T> Query<T>() where T : IEntity;

        T Load<T>(object id) where T : IEntity;

        Task<T> LoadAsync<T>(object id) where T : IEntity;

        T Get<T>(object id) where T : IEntity;

        Task<T> GetAsync<T>(object id) where T : IEntity;

        void Save(IEntity model);

        Task SaveAsync(IEntity model);

        void Delete(IEntity model);

        Task DeleteAsync(IEntity model);

        void Refresh(IEntity model);

        Task RefreshAsync(IEntity model);
    }
}
