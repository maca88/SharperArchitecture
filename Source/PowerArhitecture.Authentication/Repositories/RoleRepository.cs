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
using PowerArhitecture.Domain;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Repositories
{
    public class RoleRepository<TRole> : Repository<TRole>, IRoleRepository<TRole>
         where TRole : class, IRole, IEntity<long>, new()
    {
        public RoleRepository(ISession session, ILogger logger, ISessionEventProvider sessionEventProvider) 
            : base(session, logger, sessionEventProvider)
        {
        }

        public Task CreateAsync(TRole role)
        {
            Save(role);
            return Task.FromResult(1);
        }

        public Task UpdateAsync(TRole role)
        {
            Save(role);
            return Task.FromResult(1);
        }

        public Task DeleteAsync(TRole role)
        {
            Delete(role);
            return Task.FromResult(1);
        }

        public Task<TRole> FindByIdAsync(long roleId)
        {
            var role = Get(roleId);
            return Task.FromResult(role);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            var roles = Query().FirstOrDefault(o => o.Name == roleName);
            return Task.FromResult(roles);
        }

        public void Dispose()
        {
        }
    }

    public interface IRoleRepository<TRole> : IRoleStore<TRole, long>, IRepository<TRole>
        where TRole : class, IRole, IEntity<long>, new()
    {
        
    }
}
