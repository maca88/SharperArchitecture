using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.DataAccess.Configurations
{
    public class HiLoIdConfiguration
    {
        public virtual bool Enabled { get; set; }

        public virtual string TableName { get; set; }

        public virtual int MaxLo { get; set; }

        internal void FillFromConfig()
        {
            Enabled = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.ConventionsHiLoIdEnabled);
            TableName = AppConfiguration.GetSetting<string>(DatabaseConfigurationKeys.ConventionsHiLoIdTableName);
            MaxLo = AppConfiguration.GetSetting<int?>(DatabaseConfigurationKeys.ConventionsHiLoIdMaxLo).GetValueOrDefault();
        }
    }
}
