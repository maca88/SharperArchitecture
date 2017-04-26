using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Common.Configuration;

namespace SharperArchitecture.Authentication.Configurations
{
    public class AuthenticationConfiguration : IAuthenticationConfiguration
    {
        public AuthenticationConfiguration()
        {
            FillFromConfig();
        }

        public virtual string SystemUserPassword { get; set; }

        public virtual string SystemUserName { get; set; }

        public virtual bool Caching { get; set; }

        private void FillFromConfig()
        {
            SystemUserPassword = AppConfiguration.GetSetting<string>(AuthenticationConfigurationKeys.SystemUserPassword);
            SystemUserName = AppConfiguration.GetSetting<string>(AuthenticationConfigurationKeys.SystemUserName);
            Caching = AppConfiguration.GetSetting<bool>(AuthenticationConfigurationKeys.Caching);
        }
    }
}
