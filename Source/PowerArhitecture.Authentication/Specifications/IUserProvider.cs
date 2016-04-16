using System;
using System.Security.Principal;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Authentication.Specifications
{
    public interface IUserProvider
    {
        IUser GetCurrentUser();

        IUser GetUser(string userName);

        IUser GetSystemUser();

    }
}
