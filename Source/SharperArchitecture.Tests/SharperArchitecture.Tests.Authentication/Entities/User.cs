using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using SharperArchitecture.Authentication.Entities;

namespace SharperArchitecture.Tests.Authentication.Entities
{
    public class User :User<User, UserRole, Role, RolePermission, PermissionPattern, Organization, OrganizationRole>
    {
    }

    public class UserOverride : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.HasMany(o => o.Claims).KeyColumn("UserId");
            mapping.HasMany(o => o.Settings).KeyColumn("UserId");
        }
    }

    public class UserClaimOverride : IAutoMappingOverride<UserClaim>
    {
        public void Override(AutoMapping<UserClaim> mapping)
        {
            mapping.References(o => o.User).Class<User>();
        }
    }

    public class UserLoginOverride : IAutoMappingOverride<UserLogin>
    {
        public void Override(AutoMapping<UserLogin> mapping)
        {
            mapping.References(o => o.User).Class<User>();
        }
    }

    public class UserSettingOverride : IAutoMappingOverride<UserSetting>
    {
        public void Override(AutoMapping<UserSetting> mapping)
        {
            mapping.References(o => o.User).Class<User>();
        }
    }

    public class UserRole : UserRole<User, UserRole, Role, RolePermission, PermissionPattern, Organization, OrganizationRole>
    {

    }

    public class Role : Role<User, UserRole, Role, RolePermission, PermissionPattern, Organization, OrganizationRole>
    {
    }

    public class RolePermission : RolePermission<User, UserRole, Role, RolePermission, PermissionPattern, Organization, OrganizationRole>
    {
    }

    public class PermissionPattern : PermissionPattern<User, UserRole, Role, RolePermission, PermissionPattern, Organization, OrganizationRole>
    {
    }

    public class Organization : Organization<User, UserRole, Role, RolePermission, PermissionPattern, Organization, OrganizationRole>
    {
    }

    public class OrganizationRole : OrganizationRole<User, UserRole, Role, RolePermission, PermissionPattern, Organization, OrganizationRole>
    {
    }
}

