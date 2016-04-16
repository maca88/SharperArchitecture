using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Mapping;
using Ninject;
using Ninject.Syntax;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Authentication
{
    public class UserProvider : IUserProvider
    {
        private readonly IAuthenticationConfiguration _configuration;
        private readonly IResolutionRoot _resolutionRoot;
        private Type _userType;
        private MethodInfo _sessionUserQuery;

        public UserProvider(IResolutionRoot resolutionRoot, IAuthenticationConfiguration configuration)
        {
            _configuration = configuration;
            _resolutionRoot = resolutionRoot;
            _userType = configuration.GetUserType();
            _sessionUserQuery = typeof(LinqExtensionMethods).GetMethods()
                .Single(o => o.Name == "Query" && o.GetParameters().Length == 1 && o.GetParameters()[0].ParameterType == typeof(ISession))
                .MakeGenericMethod(_userType);
        }

        public IUser GetCurrentUser()
        {
            var identity = GetCurrentPrincipal().Identity;
            if (!identity.IsAuthenticated) return null;
            return GetUser(identity.Name);
        }

        public IPrincipal GetCurrentPrincipal()
        {
            return Thread.CurrentPrincipal;
        }

        public IUser GetUser(string userName)
        {
            var session = _resolutionRoot.Get<ISession>();
            var query = (IQueryable)_sessionUserQuery.Invoke(null, new object[] { session });
            return query.Where("Name = @0", userName).ToList<IUser>().FirstOrDefault();
        }

        public IUser GetSystemUser()
        {
            return GetUser(_configuration.SystemUserName);
        }

        public DateTimeOffset GetDateTimeOffset(DateTime userDateTime)
        {
            return new DateTimeOffset(userDateTime, GetCurrentUser().TimeZone.BaseUtcOffset);
        }
    }
}
