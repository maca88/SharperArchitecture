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
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Validation.Specifications;
using Microsoft.AspNet.Identity;
using NHibernate;
using Ninject.Extensions.Logging;

namespace PowerArhitecture.Authentication.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        
        public UserRepository(ISession session, ILogger logger, ISessionEventListener sessionEventListener) 
            : base(session, logger, sessionEventListener)
        {  
        }

        public Task CreateAsync(User user)
        {
            Save(user);
            return Task.FromResult(1);
        }

        public Task UpdateAsync(User user)
        {
            Save(user);
            return Task.FromResult(1);
        }

        public Task DeleteAsync(User user)
        {
            Delete(user);
            return Task.FromResult(1);
        }

        public Task<User> FindByIdAsync(long userId)
        {
            var user = Get(userId);
            return Task.FromResult(user);
        }

        public Task<User> FindByNameAsync(string userName)
        {
            var user = GetLinqQuery().FirstOrDefault(o => o.UserName == userName);
            return Task.FromResult(user);
        }

        public Task AddLoginAsync(User user, UserLoginInfo login)
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

        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            var item = user.Logins
                           .SingleOrDefault(o =>
                                            o.User.Id == user.Id &&
                                            o.LoginProvider == login.LoginProvider &&
                                            o.ProviderKey == login.ProviderKey);
            if (item != null)
                user.RemoveLogin(item);
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            IList<UserLoginInfo> result = user.Logins
                .Select(o => new UserLoginInfo(o.LoginProvider, o.ProviderKey))
                .ToList();
            return Task.FromResult(result);
        }

        public Task<User> FindAsync(UserLoginInfo login)
        {
            var user = Session.QueryOver<UserLogin>()
                              .Where(o => o.LoginProvider == login.LoginProvider &&
                                          o.ProviderKey == login.ProviderKey)
                              .Select(o => o.User)
                              .SingleOrDefault<User>();
            return Task.FromResult(user);
        }

        public Task<IList<Claim>> GetClaimsAsync(User user)
        {
            IList<Claim> result = user.Claims.Select(claim => new Claim(claim.ClaimType, claim.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        public Task AddClaimAsync(User user, Claim claim)
        {
            var item = new UserClaim
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
            user.AddClaim(item);
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(User user, Claim claim)
        {
            foreach (var item in user.Claims
                .Where(o => o.ClaimType == claim.Value && o.ClaimValue == claim.Value))
            {
                user.RemoveClaim(item);
            }
            return Task.FromResult(0);
        }

        public Task AddToRoleAsync(User user, string role)
        {
            var roleName = role.ToUpper();
            var r = Session.QueryOver<Role>().Where(o => o.Name == roleName).SingleOrDefault();
            if (r == null)
                throw new InvalidOperationException(string.Format("Role '{0}' not found", role ));
            var item = new Role
                {
                    Name = roleName
                };
            user.AddToRole(item);
            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(User user, string role)
        {
            user.RemoveFromRole(role);
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            IList<string> roles = user.UserRoles.Select(o => o.Role.Name).ToList();
            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(User user, string role)
        {
            var inRole = user.IsInRole(role.ToUpper());
            return Task.FromResult(inRole);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetSecurityStampAsync(User user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(User user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public void Dispose()
        {
        }
    }

    public interface IUserRepository : IRepository<User>,
        IUserLoginStore<User, long>, IUserClaimStore<User, long>, IUserRoleStore<User, long>, IUserPasswordStore<User, long>, IUserSecurityStampStore<User, long>
    {
    }
}
