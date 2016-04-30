using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Configuration
{
    public class NinjectConfiguration
    {
        public NinjectConfiguration()
        {
        }
        public virtual string XmlConfig
        {
            get { return AppConfiguration.GetSetting<string>("PowerArhitecture.Common.Ninject.XmlConfig:string"); }
        }
    }
}
