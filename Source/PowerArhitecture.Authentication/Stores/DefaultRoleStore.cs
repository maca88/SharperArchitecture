using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NHibernate.Linq;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Stores
{
    public class DefaultRoleStore<TRole> :
        IQueryableRoleStore<TRole, long>
         where TRole : class, IRole, IEntity<long>, new()
    {
        public DefaultRoleStore(IDbStore dbStore)
        {
            DbStore = dbStore;
        }

        protected IDbStore DbStore { get; }

        public Task CreateAsync(TRole role)
        {
            return DbStore.SaveAsync(role);
        }

        public Task UpdateAsync(TRole role)
        {
            return DbStore.SaveAsync(role);
        }

        public Task DeleteAsync(TRole role)
        {
            return DbStore.DeleteAsync(role);
        }

        public Task<TRole> FindByIdAsync(long roleId)
        {
            return DbStore.GetAsync<TRole>(roleId);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            return DbStore.Query<TRole>().FirstOrDefaultAsync(o => o.Name == roleName);
        }

        public IQueryable<TRole> Roles => DbStore.Query<TRole>();

        void IDisposable.Dispose()
        {
        }
    }
}
