using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Authentication.Specifications
{
    public interface IAuthenticationCache
    {
        IUser GetUser(string userName);

        void InsertOrUpdateUser(IUser user);

        void DeleteUser(IUser user);
    }
}