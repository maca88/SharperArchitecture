using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Enums;
using PowerArhitecture.Common.Extensions;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Authentication.Generated;
using PowerArhitecture.Validation.Attributes;
using Newtonsoft.Json;

namespace PowerArhitecture.Authentication.Entities
{
    [Serializable]
    [IgnoreValidationAttributes(Properties = new[] { "CreatedBy", "LastModifiedBy" })] //Overridden in class PrincipalConvention //TODO: generate metadata for entities
    public partial class User : VersionedEntity, IUser, IIdentity
    {
        public virtual ISet<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new HashSet<UserRole>()); }
            set { _userRoles = value; }
        }

        public virtual bool IsInRole(string role)
        {
            return UserRoles.Select(o => o.Role).Any(o => o.Name == role);
        }

        public virtual IIdentity Identity { get { return this; } }

        [Length(50)]
        [NotNull]
        [Unique]
        public virtual string UserName { get; set; }

        [NotNull]
        public virtual string PasswordHash { get; set; }

        public virtual string SecurityStamp { get; set; }

        public virtual string CultureName { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual CultureInfo Culture
        {
            get
            {
                return !string.IsNullOrEmpty(CultureName)
                    ? CultureInfo.GetCultureInfo(CultureName)
                    : null;
            }
        }

        public virtual ISet<UserClaim> Claims
        {
            get { return _claims ?? (_claims = new HashSet<UserClaim>()); }
            set { _claims = value; }
        }

        public virtual ISet<UserLogin> Logins
        {
            get { return _logins ?? (_logins = new HashSet<UserLogin>()); }
            set { _logins = value; }
        }

        public virtual string AuthenticationType { get { return ""; } }

        public virtual bool IsAuthenticated { get { return !IsTransient(); } }

        [NotNull]
        public virtual string TimeZoneId { get; set; }

        public virtual DateTime GetCurrentDateTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
        }

        public virtual DateTimeOffset GetCurrentDateTimeOffset()
        {
            return new DateTimeOffset(GetCurrentDateTime(), TimeZone.BaseUtcOffset);
        }

        public virtual TimeZoneInfo TimeZone
        {
            get
            {
                return string.IsNullOrEmpty(TimeZoneId) 
                    ? TimeZoneInfo.Utc
                    : TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            }
        }

        public virtual bool HasPermission(string pattern, PatternOption opt = PatternOption.None)
        {
            var permPattern = opt.ProcessPattern(pattern);
            return
                IsSystemUser ||
                (
                    Organization != null &&
                    (
                        Organization.OrganizationRoles.Select(o => o.Role).Any(o => o.RolePermissions.Any(p => Regex.IsMatch(p.Permission.Name, permPattern))) ||
                        Organization.OrganizationRoles.Select(o => o.Role).Any(o => o.PermissionPatterns.Any(p => Regex.IsMatch(pattern, p.Pattern)))
                    )
                ) ||
                UserRoles.Select(o => o.Role).Any(o => o.RolePermissions.Any(p => Regex.IsMatch(p.Permission.Name, permPattern))) ||
                UserRoles.Select(o => o.Role).Any(o => o.PermissionPatterns.Any(p => Regex.IsMatch(pattern, p.Pattern)));
        }

        public virtual bool IsSystemUser
        {
            get { return AppConfiguration.GetSetting<string>(AuthenticationSettingKeys.SystemUserName) == Name; }
        }

        public virtual string Name { get { return UserName; } }

        #region Settings

        public virtual ISet<UserSetting> Settings
        {
            get { return _settings ?? (_settings = new HashSet<UserSetting>()); }
            set { _settings = value; }
        }

        public virtual bool ContainsSetting(string name)
        {
            return Settings.Any(o => o.Name == name);
        }

        public virtual T GetSetting<T>(string name)
        {
            return ContainsSetting(name) 
                ? JsonConvert.DeserializeObject<T>(Settings.First(o => o.Name == name).Value) 
                : default(T);
        }

        public virtual void SetSetting<T>(string name, T value)
        {
            var setting = Settings.FirstOrDefault(o => o.Name == name) ??
                          new UserSetting
                          {
                              Name = name,
                              User = this
                          };
            setting.Value = JsonConvert.SerializeObject(value);
            Settings.Add(setting);
        }

        #endregion

        public virtual void AddToRole(Role item)
        {
            AddUserRole(new UserRole{Role = item});
        }

        public virtual void RemoveFromRole(Role role)
        {
            RemoveFromRole(role.Name);
        }

        public virtual void RemoveFromRole(string roleName)
        {
            var userRole = UserRoles.FirstOrDefault(o => o.Role.Name == roleName);
            if (userRole == null) return;
            RemoveUserRole(userRole);
        }
    }
}
