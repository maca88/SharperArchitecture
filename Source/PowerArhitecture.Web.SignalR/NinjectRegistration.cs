using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Ninject.Modules;

namespace PowerArhitecture.Web.SignalR
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<IDependencyResolver>().To<SignalRNinjectDependencyResolver>().InSingletonScope();
        }
    }
}
