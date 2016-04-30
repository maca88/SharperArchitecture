using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using FluentNHibernate.Automapping;
using PowerArhitecture.Authentication.Configurations;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Enums;
using PowerArhitecture.Common.Extensions;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Validation.Attributes;
using Newtonsoft.Json;

namespace PowerArhitecture.Authentication.Entities
{
    [Serializable]
    [Ignore]
    [IgnoreValidationAttributes(Properties = new[] { "CreatedBy", "LastModifiedBy" })] //Overridden in class PrincipalConvention //TODO: generate metadata for entities
    public abstract partial class User<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>
        : VersionedEntityWithUser<TUser>, IUser, IIdentity
        where TUser : User<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TUserRole : UserRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRole : Role<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, IEntity, new()
        where TRolePermission : RolePermission<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TPermissionPattern : PermissionPattern<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganization : Organization<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
        where TOrganizationRole : OrganizationRole<TUser, TUserRole, TRole, TRolePermission, TPermissionPattern, TOrganization, TOrganizationRole>, new()
    {
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

        public virtual CultureInfo Culture
        {
            get
            {
                return !string.IsNullOrEmpty(CultureName)
                    ? CultureInfo.GetCultureInfo(CultureName)
                    : null;
            }
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

        public virtual bool HasPermission(string pattern, PatternOptions opt = PatternOptions.None)
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
            get { return AppConfiguration.GetSetting<string>(AuthenticationConfigurationKeys.SystemUserName) == Name; }
        }

        [Formula("UserName")]
        public virtual string Name { get { return UserName; } set {} }

        #region LastModifiedBy

        [ReadOnly(true)]
        public virtual long? LastModifiedById
        {
            get
            {
                if (_lastModifiedByIdSet) return _lastModifiedById;
                return LastModifiedBy == null ? default(long?) : LastModifiedBy.Id;
            }
            set
            {
                _lastModifiedByIdSet = true;
                _lastModifiedById = value;
            }
        }

        private long? _lastModifiedById;

        private bool _lastModifiedByIdSet;

        #endregion

        #region CreatedBy

        [ReadOnly(true)]
        public virtual long? CreatedById
        {
            get
            {
                if (_createdByIdSet) return _createdById;
                return CreatedBy == null ? default(long?) : CreatedBy.Id;
            }
            set
            {
                _createdByIdSet = true;
                _createdById = value;
            }
        }

        private long? _createdById;

        private bool _createdByIdSet;

        #endregion

        #region UserRoles

        private ISet<TUserRole> _userRoles;

        public virtual ISet<TUserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new HashSet<TUserRole>()); }
            set { _userRoles = value; }
        }

        public virtual void AddUserRole(TUserRole userRole)
        {
            this.AddOneToMany(o => o.UserRoles, userRole, o => o.User, o => o.RemoveUserRole);
        }

        public virtual void RemoveUserRole(TUserRole userRole)
        {
            this.RemoveOneToMany(o => o.UserRoles, userRole, o => o.User);
        }

        #endregion

        #region Organization

        [ReadOnly(true)]
        public virtual long? OrganizationId
        {
            get
            {
                if (_organizationIdSet) return _organizationId;
                return Organization == null ? default(long?) : Organization.Id;
            }
            set
            {
                _organizationIdSet = true;
                _organizationId = value;
            }
        }

        private long? _organizationId;

        private bool _organizationIdSet;

        public virtual TOrganization Organization { get; set; }

        #endregion

        #region Claims

        public virtual ISet<UserClaim> Claims
        {
            get { return _claims ?? (_claims = new HashSet<UserClaim>()); }
            set { _claims = value; }
        }

        public virtual void AddClaim(UserClaim claim)
        {
            this.AddOneToMany(o => o.Claims, claim, o => (TUser)o.User, o => o.RemoveClaim);
        }

        public virtual void RemoveClaim(UserClaim claim)
        {
            this.RemoveOneToMany(o => o.Claims, claim, o => (TUser)o.User);
        }

        public virtual IEnumerable<UserClaim> GetAllClaims()
        {
            return Claims;
        }

        #endregion

        #region Logins

        public virtual ISet<UserLogin> Logins
        {
            get { return _logins ?? (_logins = new HashSet<UserLogin>()); }
            set { _logins = value; }
        }

        public virtual void AddLogin(UserLogin login)
        {
            this.AddOneToMany(o => o.Logins, login, o => (TUser)o.User, o => o.RemoveLogin);
        }

        public virtual IEnumerable<UserLogin> GetAllLogins()
        {
            return Logins;
        }

        public virtual void RemoveLogin(UserLogin login)
        {
            this.RemoveOneToMany(o => o.Logins, login, o => (TUser)o.User);
        }

        #endregion

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

        public virtual void AddSetting(UserSetting setting)
        {
            this.AddOneToMany(o => o.Settings, setting, o => (TUser)o.User, o => o.RemoveSetting);
        }

        public virtual void RemoveSetting(UserSetting setting)
        {
            this.RemoveOneToMany(o => o.Settings, setting, o => (TUser)o.User);
        }

        #endregion

        public virtual void AddToRole(IRole role)
        {
            AddToRole((TRole)role);
        }

        public virtual IEnumerable<IRole> GetRoles()
        {
            return UserRoles.Select(o => o.Role);
        }

        public virtual void AddToRole(TRole item)
        {
            AddUserRole(new TUserRole { Role = item });
        }

        public virtual void RemoveFromRole(TRole role)
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
