using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.Authentication.Configurations
{
    public class AuthenticationConfiguration : IAuthenticationConfiguration
    {
        public AuthenticationConfiguration()
        {
            FillFromConfig();
        }

        public virtual string SystemUserPassword { get; set; }

        public virtual string SystemUserName { get; set; }

        public virtual string UserClass { get; set; }

        public virtual bool Caching { get; set; }

        public Type GetUserType()
        {
            return Type.GetType(UserClass, true);
        }

        private void FillFromConfig()
        {
            SystemUserPassword = AppConfiguration.GetSetting<string>(AuthenticationConfigurationKeys.SystemUserPassword);
            SystemUserName = AppConfiguration.GetSetting<string>(AuthenticationConfigurationKeys.SystemUserName);
            UserClass = AppConfiguration.GetSetting<string>(AuthenticationConfigurationKeys.UserClass);
            Caching = AppConfiguration.GetSetting<bool>(AuthenticationConfigurationKeys.Caching);
        }
    }
}
