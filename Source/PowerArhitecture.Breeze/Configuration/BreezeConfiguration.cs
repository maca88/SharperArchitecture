using PowerArhitecture.Breeze.Specification;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.Breeze.Configuration
{
    public class BreezeConfiguration : IBreezeConfiguration
    {
        public virtual string DataServiceNamesBaseUri
        {
            get { return AppConfiguration.GetSetting<string>(BreezeConfigurationKeys.DataServiceNamesBaseUri); }
        }
    }
}
