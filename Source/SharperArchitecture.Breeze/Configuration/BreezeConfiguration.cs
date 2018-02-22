using System;
using SharperArchitecture.Breeze.Specification;
using SharperArchitecture.Common.Configuration;

namespace SharperArchitecture.Breeze.Configuration
{
    public class BreezeConfiguration : IBreezeConfiguration
    {
        public virtual string DataServiceNamesBaseUri
        {
            get { return AppConfiguration.GetSetting<string>(BreezeConfigurationKeys.DataServiceNamesBaseUri); }
        }

        public virtual Type ClientModelAttribute { get; private set; }
    }
}
