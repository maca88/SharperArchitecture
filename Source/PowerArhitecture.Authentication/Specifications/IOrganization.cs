using System.Collections.Generic;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain.Specifications;

namespace PowerArhitecture.Authentication.Specifications
{
    public interface IOrganization : IVersionedEntity<IUser>
    {
        ISet<OrganizationRole> OrganizationRoles { get; }

        void AddOrganizationRole(OrganizationRole organizationRole);

        void RemoveOrganizationRole(OrganizationRole organizationRole);
    }
}