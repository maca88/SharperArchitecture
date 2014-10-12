using System.Security.Principal;
using BAF.Authentication.Specifications;
using BAF.Common.Helpers;
using BAF.Authentication.Specifications;
using FluentValidation.Results;

namespace BAF.Authentication.Authentications
{/*
    public class DummyAuthentication : IAuthentication
    {
        public ValidationFailure LogOn(string username, string password)
        {
            return username == "test" 
                ? null
                : new ValidationFailure("", "Incorrect username or password");
        }

        public TUser GetCurrentUser<TUser>() where TUser : class, IPrincipal
        {
            return PrincipalHelper.GetCurrentUser() as TUser;
        }

        public void LogOut()
        {
        }
    }*/
}
