using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.DataAccess.Configurations
{
    public class ConventionsConfiguration
    {
        public ConventionsConfiguration()
        {
            HiLoId = new HiLoIdConfiguration();
        }

        public virtual HiLoIdConfiguration HiLoId { get; }
        public virtual bool IdDescending { get; internal set; }
        public virtual bool UniqueWithMultipleNulls { get; internal set; }
        public virtual bool RequiredLastModifiedProperty { get; internal set; }
        public virtual string DateTimeZone { get; internal set; }

        internal void FillFromConfig()
        {
            IdDescending = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.ConventionsIdDescending);
            UniqueWithMultipleNulls = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.ConventionsUniqueWithMultipleNulls);
            RequiredLastModifiedProperty = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.ConventionsRequiredLastModifiedProperty);
            DateTimeZone = AppConfiguration.GetSetting<string>(DatabaseConfigurationKeys.ConventionsDateTimeZone);
            HiLoId.FillFromConfig();
        }
    }
}
