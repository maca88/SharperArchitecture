using System.Security.Principal;
using FluentValidation.Results;

namespace BAF.Authentication.Specifications
{
    public interface IAuthentication
    {
        ValidationFailure LogOn(string name, string password);

        TUser GetCurrentUser<TUser>() where TUser : class, IPrincipal;

        void LogOut();
    }
}
