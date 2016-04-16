using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Enums;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Specifications
{
    public interface IUser : IPrincipal, IUser<long>, IEntity
    {
        bool HasPermission(string pattern, PatternOptions opt = PatternOptions.None);

        bool IsSystemUser { get; }

        DateTime GetCurrentDateTime();

        DateTimeOffset GetCurrentDateTimeOffset();

        TimeZoneInfo TimeZone { get; }

        bool ContainsSetting(string name);

        T GetSetting<T>(string name);

        void AddLogin(UserLogin login);

        void RemoveLogin(UserLogin login);

        IEnumerable<UserLogin> GetAllLogins();

        void AddClaim(UserClaim claim);

        void RemoveClaim(UserClaim claim);

        IEnumerable<UserClaim> GetAllClaims();

        void AddToRole(IRole role);

        void RemoveFromRole(string roleName);

        IEnumerable<IRole> GetRoles();

        string PasswordHash { get; set; }

        string SecurityStamp { get; set; }
    }
}