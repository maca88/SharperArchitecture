using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Configuration
{
    public partial class CommonConfiguration : Specifications.ICommonConfiguration
    {
        public virtual string DefaultCulture
        {
            get { return AppConfiguration.GetSetting<string>(CommonConfigurationKeys.DefaultCulture); }
        }
        public virtual string TranslationsByCulturePattern
        {
            get { return AppConfiguration.GetSetting<string>(CommonConfigurationKeys.TranslationsByCulturePattern); }
        }
        public virtual string TranslationsPath
        {
            get { return AppConfiguration.GetSetting<string>(CommonConfigurationKeys.TranslationsPath); }
        }
    }
}
