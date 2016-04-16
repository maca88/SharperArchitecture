using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using PowerArhitecture.Notifications.Specifications;
using PowerArhitecture.Tests.Notifications.SearchQueries;

namespace PowerArhitecture.Tests.Notifications
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<IRecipientSearchQuery>().To<TestStringRecipientSeachQuery>().Named("String.Test");
            Bind<IRecipientSearchQuery>().To<TestEntityRecipientSeachQuery>().Named("Entity.Test");
        }
    }
}
