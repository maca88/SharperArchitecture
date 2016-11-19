using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.CodeList.Configurations;
using PowerArhitecture.CodeList.Specifications;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace PowerArhitecture.CodeList
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<ICodeListConfiguration, CodeListConfiguration>();
        }
    }
}
