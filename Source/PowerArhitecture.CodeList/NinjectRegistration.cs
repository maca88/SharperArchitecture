using System.Linq;
using PowerArhitecture.CodeList.Configurations;
using PowerArhitecture.CodeList.Specifications;
using Ninject.Modules;

namespace PowerArhitecture.CodeList
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            //Bind<ICodeListCache, IApplicationCache>().To<CodeListCache>().InSingletonScope();
            if (!Kernel.GetBindings(typeof(ICodeListConfiguration)).Any())
                Bind<ICodeListConfiguration>().To<CodeListConfiguration>().InSingletonScope();
        }
    }
}
