using System;
using System.Collections.Generic;
using System.Security.Principal;
using PowerArhitecture.Common.Enums;
using Microsoft.AspNet.Identity;

namespace PowerArhitecture.Common.Specifications
{
    public interface IUser : IPrincipal, IUser<long>
    {
        bool HasPermission(string pattern, PatternOption opt = PatternOption.None);

        bool IsSystemUser { get; }

        DateTime GetCurrentDateTime();

        DateTimeOffset GetCurrentDateTimeOffset();

        TimeZoneInfo TimeZone { get; }

        bool ContainsSetting(string name);

        T GetSetting<T>(string name);

        //void SetSetting<T>(string name, T value);
    }
}
