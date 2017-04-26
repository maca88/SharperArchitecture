using Microsoft.AspNet.Identity;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Authentication.Specifications
{
    public interface IRole : IRole<long>, IEntity
    {
        string Description { get; set; }

        void AddUser(IUser user);

        void RemoveUser(IUser user);
    }
}
