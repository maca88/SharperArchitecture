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
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using PowerArhitecture.Domain;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Repositories
{
    public class RoleRepository<TRole> : Repository<TRole>, IRoleRepository<TRole>
         where TRole : class, IRole, IEntity<long>, new()
    {
        public RoleRepository(ISession session, ILogger logger) 
            : base(session, logger)
        {
        }

        public Task CreateAsync(TRole role)
        {
            return SaveAsync(role);
        }

        public Task<TRole> FindByIdAsync(long roleId)
        {
            return GetAsync(roleId);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            return Query().FirstOrDefaultAsync(o => o.Name == roleName);
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
