using System;
using System.Threading;
using System.Web;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Authentication.HttpModule
{
    public class PrincipalHttpModule : IHttpModule
    {
        private readonly IUserProvider _userProvider;

        public PrincipalHttpModule(IUserProvider userProvider)
        {
            _userProvider = userProvider;
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
            var user = _userProvider.GetUser(identity.Name);
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