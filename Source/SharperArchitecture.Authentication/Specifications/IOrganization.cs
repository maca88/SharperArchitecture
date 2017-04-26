using System.Collections.Generic;
using SharperArchitecture.Authentication.Entities;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Domain.Specifications;

namespace SharperArchitecture.Authentication.Specifications
{
    public interface IOrganization<TUser> : IVersionedEntityWithUser<TUser>, IOrganization
    {
    }

    public interface IOrganization : IVersionedEntity
    {
    }
}