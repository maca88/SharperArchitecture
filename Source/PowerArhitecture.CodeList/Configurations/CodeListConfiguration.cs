using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.CodeList.Configurations
{
    public class CodeListConfiguration : ICodeListConfiguration
    {
        public CodeListConfiguration()
        {
            FillFromConfig();
        }

        public bool EnableCache { get; set; }

        private void FillFromConfig()
        {
            EnableCache = AppConfiguration.GetSetting<bool>(CodeListConfigurationKeys.EnableCache);
        }
    }
}
