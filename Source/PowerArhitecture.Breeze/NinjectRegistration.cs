using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Breeze.Specification;
using PowerArhitecture.Common.Specifications;
using Breeze.ContextProvider.NH;
using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Conventions;

namespace PowerArhitecture.Breeze
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<BreezeMetadataConfigurator>().ToSelf().InSingletonScope();
            Bind<IBreezeRepository>().To<BreezeRepository>();
            Kernel.Bind(o => o
                .From(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetTypes().Any(t => typeof(IBreezeModelConfigurator).IsAssignableFrom(t))))
                .IncludingNonePublicTypes()
                .SelectAllClasses()
                .InheritedFrom<IBreezeModelConfigurator>()
                .WhichAreNotGeneric()
                .BindAllInterfaces()
                .Configure(c => c.InSingletonScope()));
            //BreezeModelConfigurator.SerializationMemberRuleCreated += (rule =>
            //{
            //    rule.Data["IUserCache"] = Kernel.Get<IUserCache>();
            //});
            //BreezeModelConfigurator.ModelConfigurationCreated += (rule =>
            //{
            //    rule.Data["IUserCache"] = Kernel.Get<IUserCache>();
            //});
        }
    }
}
