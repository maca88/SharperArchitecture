using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.DataCaching.Specifications;
using Ninject.Modules;

namespace PowerArhitecture.CodeList
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            //Bind<ICodeListCache, IApplicationCache>().To<CodeListCache>().InSingletonScope();
        }
    }
}
