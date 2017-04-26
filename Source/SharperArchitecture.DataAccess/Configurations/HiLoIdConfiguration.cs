using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Configuration;

namespace SharperArchitecture.DataAccess.Configurations
{
    public class HiLoIdConfiguration
    {
        public virtual bool Enabled { get; internal set; }

        public virtual string TableName { get; internal set; }

        public virtual int MaxLo { get; internal set; }

        internal void FillFromConfig()
        {
            Enabled = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.ConventionsHiLoIdEnabled);
            TableName = AppConfiguration.GetSetting<string>(DatabaseConfigurationKeys.ConventionsHiLoIdTableName);
            MaxLo = AppConfiguration.GetSetting<int?>(DatabaseConfigurationKeys.ConventionsHiLoIdMaxLo).GetValueOrDefault();
        }
    }
}
