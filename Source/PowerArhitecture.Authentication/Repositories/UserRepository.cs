using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using IUser = PowerArhitecture.Authentication.Specifications.IUser;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Repositories
{
    //[Repository(AutoBind = false)]
    public abstract class UserRepository<TUser, TRole> : Repository<TUser>, IUserRepository<TUser>
        where TUser : class, IUser, IEntity<long>, new()
        where TRole : class, IRole, IEntity<long>, new()
    {
        protected readonly IAuthenticationConfiguration Configuration;

        protected UserRepository(ISession session, ILogger logger, IAuthenticationConfiguration configuration) 
            : base(session, logger)
        {
            Configuration = configuration;
        }

        public virtual Task CreateAsync(TUser user)
        {
            return SaveAsync(user);
        }

        public virtual Task<TUser> FindByIdAsync(long userId)
        {
            return GetAsync(userId);
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            var query =  Query();
            if (Configuration.Caching)
                query = query.Cacheable();
            return query.FirstOrDefaultAsync(o => o.UserName == userName);
        }

        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            var item = new UserLogin
            {
                User = user,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            };
            user.AddLogin(item);
            return Task.FromResult(0);
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            var id = ((IUser<long>)user).Id;
            var item = user.GetAllLogins()
                           .SingleOrDefault(o =>
                                            o.User.Id == id &&
                                            o.LoginProvider == login.LoginProvider &&
                                            o.ProviderKey == login.ProviderKey);
            if (item != null)
                user.RemoveLogin(item);
            return Task.FromResult(0);
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            IList<UserLoginInfo> result = user.GetAllLogins()
                .Select(o => new UserLoginInfo(o.LoginProvider, o.ProviderKey))
                .ToList();
            return Task.FromResult(result);
        }

        public virtual async Task<TUser> FindAsync(UserLoginInfo login)
        {
            return (TUser) await Query<UserLogin>()
                              .Where(o => o.LoginProvider == login.LoginProvider &&
                                          o.ProviderKey == login.ProviderKey)
                              .Select(o => o.User)
                              .SingleOrDefaultAsync();
        }

        public virtual Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            IList<Claim> result = user.GetAllClaims().Select(claim => new Claim(claim.ClaimType, claim.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        public virtual Task AddClaimAsync(TUser user, Claim claim)
        {
            var item = new UserClaim
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
            user.AddClaim(item);
            return Task.FromResult(0);
        }

        public virtual Task RemoveClaimAsync(TUser user, Claim claim)
        {
            foreach (var item in user.GetAllClaims()
                .Where(o => o.ClaimType == claim.Value && o.ClaimValue == claim.Value))
            {
                user.RemoveClaim(item);
            }
            return Task.FromResult(0);
        }

        public virtual async Task AddToRoleAsync(TUser user, string role)
        {
            var roleName = role.ToUpper();
            var r = await Query<TRole>().Where(o => o.Name == roleName).SingleOrDefaultAsync();
            if (r == null)
                throw new InvalidOperationException(string.Format("Role '{0}' not found", role ));
            r.AddUser(user);
        }

        public virtual Task RemoveFromRoleAsync(TUser user, string role)
        {
            user.RemoveFromRole(role);
            return Task.FromResult(0);
        }

        public virtual Task<IList<string>> GetRolesAsync(TUser user)
        {
            IList<string> roles = user.GetRoles().Select(o => o.Name).ToList();
            return Task.FromResult(roles);
        }

        public virtual Task<bool> IsInRoleAsync(TUser user, string role)
        {
            var inRole = user.IsInRole(role.ToUpper());
            return Task.FromResult(inRole);
        }

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public virtual async Task<TUser> GetUserAsync(string userName)
        {
            return (TUser)await Session.CreateCriteria(typeof(TUser))
                .Add(Restrictions.Eq("UserName", userName))
                .SetCacheable(Configuration.Caching)
                .UniqueResultAsync();
        }

        //public virtual async Task<TUser> GetCurrentAsync()
        //{
        //    var userName = User?.Identity?.Name;
        //    if (string.IsNullOrEmpty(userName))
        //        return null;
        //    return await GetUserAsync(userName);
        //}

        public virtual Task<TUser> GetSystemUserAsync()
        {
            return GetUserAsync(Configuration.SystemUserName);
        }

        public virtual void Dispose()
        {
        }
    }

    public interface IUserRepository<TUser>
        : IRepository<TUser>, IUserLoginStore<TUser, long>, IUserClaimStore<TUser, long>, IUserRoleStore<TUser, long>,
        IUserPasswordStore<TUser, long>, IUserSecurityStampStore<TUser, long>
        where TUser : class, IUser, IEntity<long>, new()
    {
        //Task<TUser> GetCurrentAsync();

        Task<TUser> GetSystemUserAsync();

        Task<TUser> GetUserAsync(string userName);
    }
}
