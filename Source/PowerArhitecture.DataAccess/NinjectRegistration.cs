using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Ninject.Planning.Bindings;
using PowerArhitecture.Common.Extensions;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.EventListeners;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.DataAccess.Factories;
using PowerArhitecture.DataAccess.NHEventListeners;
using PowerArhitecture.DataAccess.Providers;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Web.Common;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.DataAccess.Parameters;

namespace PowerArhitecture.DataAccess
{
    public class NinjectRegistration : NinjectModule
    {
        

        public override void Load()
        {
            Bind<IUnitOfWorkFactory>().To<UnitOfWorkFactory>();
            Bind<IUnitOfWork, IUnitOfWorkImplementor>()
                .To<UnitOfWork>()
                .WithConstructorArgument("isolationLevel", (ctx, target) =>
                {
                    var level = IsolationLevel.Unspecified;
                    var attr = (IsolationLevelAttribute) target?.GetCustomAttributes(typeof(IsolationLevelAttribute), true).FirstOrDefault();
                    if (attr != null)
                    {
                        level = attr.Level;
                    }
                    return level;
                })
                .DefinesNamedScope(ResolutionScopes.UnitOfWork);

            //Events
            Bind<ISaveOrUpdateEventListener>().To<NhSaveOrUpdateEventListener>().InSingletonScope();
            Bind<ISaveOrUpdateEventListener>().To<NhUpdateEventListener>().InSingletonScope();
            Bind<ISaveOrUpdateEventListener>().To<NhSaveEventListener>().InSingletonScope();

            Bind<IPreInsertEventListener, IPreUpdateEventListener>()
                .To<EntityPreUpdateInsertEventListener>()
                .InSingletonScope();

            Bind<IPreUpdateEventListener>()
                .To<AuditEntityEventListener>()
                .InSingletonScope();
            //Bind<IPreUpdateEventListener, IPreInsertEventListener>()
            //    .To<RootAggregatePreUpdateInsertEventListener>()
            //    .InSingletonScope();

            if (!Kernel.GetBindings(typeof(IAuditUserProvider)).Any()) //bind if it was not configured in xml
                Bind<IAuditUserProvider>().To<AuditUserProvider>().InSingletonScope();
        }
    }
}
