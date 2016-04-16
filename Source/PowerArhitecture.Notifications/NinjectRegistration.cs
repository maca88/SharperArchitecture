using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Notifications.Specifications;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.BindingGenerators;
using Ninject.Modules;
using Ninject.Syntax;

namespace PowerArhitecture.Notifications
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            //Kernel.RegisterSearchQueryManager<RoleSearchQuery>("EqualsRoleName");
            
        }
    }
}
