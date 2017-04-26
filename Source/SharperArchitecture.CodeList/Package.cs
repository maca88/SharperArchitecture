using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.CodeList.Configurations;
using SharperArchitecture.CodeList.Specifications;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace SharperArchitecture.CodeList
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<ICodeListConfiguration, CodeListConfiguration>();
        }
    }
}
