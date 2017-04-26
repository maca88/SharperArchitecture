using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.CodeList.Specifications;
using SharperArchitecture.Common.Configuration;

namespace SharperArchitecture.CodeList.Configurations
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
            //EnableCache = AppConfiguration.GetSetting<bool>(CodeListConfigurationKeys.EnableCache);
        }
    }
}
