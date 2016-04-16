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
using Ninject.Extensions.Logging;
using IUser = PowerArhitecture.Authentication.Specifications.IUser;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Repositories
{
    //[Repository(AutoBind = false)]
    public abstract class UserRepository<TUser, TRole, TOrganization> : Repository<TUser>, IUserRepository<TUser>
        where TOrganization : class, IOrganization, new()
        where TUser : class, IUser, IEntity<long>, new()
        where TRole : class, IRole, IEntity<long>, new()
    {
        
        protected UserRepository(ISession session, ILogger logger, ISessionEventProvider sessionEventProvider) 
            : base(session, logger, sessionEventProvider)
        {  
        }

        public Task CreateAsync(TUser user)
        {
            Save(user);
            return Task.FromResult(1);
        }

        public Task UpdateAsync(TUser user)
        {
            Save(user);
            return Task.FromResult(1);
        }

        public Task DeleteAsync(TUser user)
        {
            Delete(user);
            return Task.FromResult(1);
        }

        public Task<TUser> FindByIdAsync(long userId)
        {
            var user = Get(userId);
            return Task.FromResult(user);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            var user = Query().FirstOrDefault(o => o.UserName == userName);
            return Task.FromResult(user);
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
            return Task.FromResult(0);
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
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            IList<UserLoginInfo> result = user.GetAllLogins()
                .Select(o => new UserLoginInfo(o.LoginProvider, o.ProviderKey))
                .ToList();
            return Task.FromResult(result);
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            var user = Session.QueryOver<UserLogin>()
                              .Where(o => o.LoginProvider == login.LoginProvider &&
                                          o.ProviderKey == login.ProviderKey)
                              .Select(o => o.User)
                              .SingleOrDefault<TUser>();
            return Task.FromResult(user);
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
            return Task.FromResult(0);
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

        public Task AddToRoleAsync(TUser user, string role)
        {
            var roleName = role.ToUpper();
            var r = Session.QueryOver<TRole>().Where(o => o.Name == roleName).SingleOrDefault();
            if (r == null)
                throw new InvalidOperationException(string.Format("Role '{0}' not found", role ));
            r.AddUser(user);
            return Task.FromResult(0);
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

        public void Dispose()
        {
        }
    }

    public interface IUserRepository<TUser>
        : IRepository<TUser>, IUserLoginStore<TUser, long>, IUserClaimStore<TUser, long>, IUserRoleStore<TUser, long>,
        IUserPasswordStore<TUser, long>, IUserSecurityStampStore<TUser, long>
        where TUser : class, IUser, IEntity<long>, new()
    {
    }
}
