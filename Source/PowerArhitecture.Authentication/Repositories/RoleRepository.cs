using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Validation.Specifications;
using Microsoft.AspNet.Identity;
using NHibernate;
using Ninject.Extensions.Logging;

namespace PowerArhitecture.Authentication.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ISession session, ILogger logger, ISessionEventListener sessionEventListener) 
            : base(session, logger, sessionEventListener)
        {
        }

        public Task CreateAsync(Role role)
        {
            Save(role);
            return Task.FromResult(1);
        }

        public Task UpdateAsync(Role role)
        {
            Save(role);
            return Task.FromResult(1);
        }

        public Task DeleteAsync(Role role)
        {
            Delete(role);
            return Task.FromResult(1);
        }

        public Task<Role> FindByIdAsync(long roleId)
        {
            var role = Get(roleId);
            return Task.FromResult(role);
        }

        public Task<Role> FindByNameAsync(string roleName)
        {
            var roles = GetLinqQuery().FirstOrDefault(o => o.Name == roleName);
            return Task.FromResult(roles);
        }

        public void Dispose()
        {
        }
    }

    public interface IRoleRepository : IRoleStore<Role, long>, IRepository<Role>
    {
        
    }
}
