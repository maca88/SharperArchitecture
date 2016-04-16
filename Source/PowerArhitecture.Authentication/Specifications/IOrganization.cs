using System.Collections.Generic;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain.Specifications;

namespace PowerArhitecture.Authentication.Specifications
{
    public interface IOrganization<TUser> : IVersionedEntityWithUser<TUser>, IOrganization
    {
    }

    public interface IOrganization : IVersionedEntity
    {
        //ISet<OrganizationRole> OrganizationRoles { get; }

        //void AddOrganizationRole(OrganizationRole organizationRole);

        //void RemoveOrganizationRole(OrganizationRole organizationRole);
    }
}