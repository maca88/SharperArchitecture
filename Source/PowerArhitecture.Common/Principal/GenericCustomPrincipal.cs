using System;
using System.Security.Principal;
using PowerArhitecture.Common.Enums;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Principal
{
    public class GenericCustomPrincipal : IUser
    {
        private readonly IPrincipal _principal;

        public GenericCustomPrincipal(IPrincipal principal)
        {
            _principal = principal;
        }

        public long Id
        {
            get { return default(long); }
        }

        public string UserName { get { return ""; } set { return;} }

        public bool HasPermission(string pattern, PatternOptions opt = PatternOptions.None)
        {
            return false;
        }

        public bool IsSystemUser
        {
            get { return false; }
        }

        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }

        public DateTimeOffset GetCurrentDateTimeOffset()
        {
            return new DateTimeOffset(DateTime.UtcNow);
        }

        public TimeZoneInfo TimeZone { get { return TimeZoneInfo.Utc; } }

        public bool ContainsSetting(string name)
        {
            return false;
        }

        public T GetSetting<T>(string name)
        {
            throw new NotSupportedException();
        }

        public void SetSetting<T>(string name, T value)
        {
            throw new NotSupportedException();
        }

        public bool IsInRole(string role)
        {
            return _principal.IsInRole(role);
        }

        public IIdentity Identity { get { return _principal.Identity; } }
    }
}
