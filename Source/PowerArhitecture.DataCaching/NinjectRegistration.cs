using System;
using System.Collections.Generic;
using System.Linq;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataCaching.DataCaches;
using PowerArhitecture.DataCaching.Specifications;
using Ninject.Modules;
using Ninject.Extensions.Conventions;

namespace PowerArhitecture.DataCaching
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            if (!Kernel.GetBindings(typeof(IDataCache)).Any()) //bind a simple in-proc data cache if it was not configured in xml 
                Bind<IDataCache>().To<SimpleDataCache>().InSingletonScope();

            //Convenction for custom repositories
            Kernel.Bind(o => o
                .From(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetTypes().Any(t => typeof(IApplicationCache).IsAssignableFrom(t))))
                .IncludingNonePublicTypes()
                .SelectAllClasses()
                .InheritedFrom<BaseApplicationCache>()
                .WhichAreNotGeneric()
                .Where(t => !Kernel.GetBindings(t).Any())
                .BindSelection((type, types) => types.Union(new List<Type>{type}))
                .Configure(syntax => syntax.InSingletonScope()));
        }
    }
}
