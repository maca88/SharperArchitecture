using System;
using System.Threading;
using System.Web;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Authentication.HttpModule
{
    public class PrincipalHttpModule : IHttpModule
    {
        private readonly IUserCache _userCache;

        public PrincipalHttpModule(IUserCache userCache)
        {
            _userCache = userCache;
        }

        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += OnPostAuthenticateRequest;
        }

        private void OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            var httpApp = (HttpApplication) sender;
            var identity = httpApp.User.Identity;
            if (!identity.IsAuthenticated)
                return;
            var user = _userCache.GetUser(identity.Name);
            if (user == null)
                throw new NullReferenceException(string.Format("Cannot find authenticated user with username '{0}'",
                    identity.Name));

            Thread.CurrentPrincipal = user;
            httpApp.Context.User = user;
        }

        public void Dispose()
        {
        }
    }
}