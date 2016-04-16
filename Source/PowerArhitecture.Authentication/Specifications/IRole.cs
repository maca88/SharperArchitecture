using Microsoft.AspNet.Identity;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Specifications
{
    public interface IRole : IRole<long>, IEntity
    {
        string Description { get; set; }

        void AddUser(IUser user);

        void RemoveUser(IUser user);
    }
}
