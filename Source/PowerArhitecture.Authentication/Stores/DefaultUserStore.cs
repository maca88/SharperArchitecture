using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NHibernate.Linq;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using IUser = PowerArhitecture.Authentication.Specifications.IUser;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Stores
{
    public class DefaultUserStore<TUser> :
        IUserLoginStore<TUser, long>,
        IUserClaimStore<TUser, long>,
        IUserRoleStore<TUser, long>,
        IUserPasswordStore<TUser, long>,
        IQueryableUserStore<TUser, long>,
        IUserSecurityStampStore<TUser, long>
        where TUser : class, IUser, IEntity<long>, new()
    {
        protected readonly IAuthenticationConfiguration Configuration;
        
        public DefaultUserStore(IDbStore dbStore, IAuthenticationConfiguration configuration)
        {
            Configuration = configuration;
            DbStore = dbStore;
        }

        protected IDbStore DbStore { get; }

        public Task CreateAsync(TUser user)
        {
            return DbStore.SaveAsync(user);
        }

        public Task UpdateAsync(TUser user)
        {
            return DbStore.SaveAsync(user);
        }

        public Task DeleteAsync(TUser user)
        {
            return DbStore.DeleteAsync(user);
        }

        public Task<TUser> FindByIdAsync(long userId)
        {
            return DbStore.GetAsync<TUser>(userId);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            var query = DbStore.Query<TUser>();
            if (Configuration.Caching)
                query = query.Cacheable();
            return query.FirstOrDefaultAsync(o => o.UserName == userName);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            var item = new UserLogin
            {
                User = user,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            };
            user.AddLogin(item);
            return DbStore.SaveAsync(item);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            var id = ((IUser<long>)user).Id;
            var item = user.GetAllLogins()
                           .SingleOrDefault(o =>
                                            o.User.Id == id &&
                                            o.LoginProvider == login.LoginProvider &&
                                            o.ProviderKey == login.ProviderKey);
            if (item != null)
                user.RemoveLogin(item);
            return DbStore.DeleteAsync(item);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            IList<UserLoginInfo> result = user.GetAllLogins()
                .Select(o => new UserLoginInfo(o.LoginProvider, o.ProviderKey))
                .ToList();
            return Task.FromResult(result);
        }

        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            return (TUser) await DbStore.Query<UserLogin>()
                              .Where(o => o.LoginProvider == login.LoginProvider &&
                                          o.ProviderKey == login.ProviderKey)
                              .Select(o => o.User)
                              .SingleOrDefaultAsync();
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            IList<Claim> result = user.GetAllClaims().Select(claim => new Claim(claim.ClaimType, claim.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            var item = new UserClaim
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
            user.AddClaim(item);
            return DbStore.SaveAsync(item);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            foreach (var item in user.GetAllClaims()
                .Where(o => o.ClaimType == claim.Value && o.ClaimValue == claim.Value))
            {
                user.RemoveClaim(item);
            }
            return Task.FromResult(0);
        }

        public async Task AddToRoleAsync(TUser user, string role)
        {
            var roleName = role.ToUpper();
            var type = Package.RoleType;
            var query = (IQueryable<IRole>) ExpressionHelper.GetMethodInfo<IDbStore>(o => o.Query<TUser>())
                .GetGenericMethodDefinition()
                .MakeGenericMethod(type)
                .Invoke(DbStore, null);
            var r = await query.Where(o => o.Name == roleName).FirstOrDefaultAsync();
            if (r == null)
                throw new InvalidOperationException($"Role '{role}' not found");
            r.AddUser(user);
        }

        public Task RemoveFromRoleAsync(TUser user, string role)
        {
            user.RemoveFromRole(role);
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            IList<string> roles = user.GetRoles().Select(o => o.Name).ToList();
            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(TUser user, string role)
        {
            var inRole = user.IsInRole(role.ToUpper());
            return Task.FromResult(inRole);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task<TUser> GetUserAsync(string userName)
        {
            var query = DbStore.Query<TUser>()
                .Where(o => o.UserName == userName);
            if (Configuration.Caching)
            {
                query = query.Cacheable();
            }
            return query.FirstOrDefaultAsync();
        }

        public Task<TUser> GetSystemUserAsync()
        {
            return GetUserAsync(Configuration.SystemUserName);
        }

        public IQueryable<TUser> Users => DbStore.Query<TUser>();

        void IDisposable.Dispose()
        {
        }
    }
}
