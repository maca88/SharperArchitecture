using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Configuration
{
    public partial class CommonConfiguration : Specifications.ICommonConfiguration
    {
        public virtual string DefaultCulture
        {
            get { return AppConfiguration.GetSetting<string>("PowerArhitecture.Common.DefaultCulture:string"); }
        }
        public virtual string TranslationsByCulturePattern
        {
            get { return AppConfiguration.GetSetting<string>("PowerArhitecture.Common.TranslationsByCulturePattern:string"); }
        }
        public virtual string DefaultTranslationsPath
        {
            get { return AppConfiguration.GetSetting<string>("PowerArhitecture.Common.DefaultTranslationsPath:string"); }
        }
    }
}
