using System;
using System.Security.Principal;
using System.Threading;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Authentication
{
    public class UserCache : IUserCache
    {
        private readonly IAuthenticationCache _authenticationCache;

        public UserCache(IAuthenticationCache cache)
        {
            _authenticationCache = cache;
        }

        public IUser GetCurrentUser()
        {
            var identity = GetCurrentPrincipal().Identity;
            if (!identity.IsAuthenticated) return null;
            return _authenticationCache.GetUser(identity.Name);
        }

        public IPrincipal GetCurrentPrincipal()
        {
            return Thread.CurrentPrincipal;
        }

        public IUser GetUser(string userName)
        {
            return _authenticationCache.GetUser(userName);
        }

        public DateTimeOffset GetDateTimeOffset(DateTime userDateTime)
        {
            return new DateTimeOffset(userDateTime, GetCurrentUser().TimeZone.BaseUtcOffset);
        }
    }
}
