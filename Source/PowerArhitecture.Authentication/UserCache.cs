using System;
using System.Security.Principal;
using System.Threading;
using NHibernate.Mapping;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Authentication
{
    public class UserCache : IUserCache
    {
        private readonly IAuthenticationCache _authenticationCache;
        private readonly IAuthenticationSettings _settings;

        public UserCache(IAuthenticationCache cache, IAuthenticationSettings settings)
        {
            _authenticationCache = cache;
            _settings = settings;
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

        public IUser GetSystemUser()
        {
            return GetUser(_settings.SystemUserName);
        }

        public DateTimeOffset GetDateTimeOffset(DateTime userDateTime)
        {
            return new DateTimeOffset(userDateTime, GetCurrentUser().TimeZone.BaseUtcOffset);
        }
    }
}
