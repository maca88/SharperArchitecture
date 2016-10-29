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
using Ninject.Web.Common;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.DataAccess.Parameters;

namespace PowerArhitecture.DataAccess
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            //Default configuration
            //var dbSettings = FluentDatabaseConfiguration.Create(new Configuration().Configure())
            //    .FillFromConfig()
            //    .Build();
            //this.RegisterDatabaseConfiguration(dbSettings);

            Bind(typeof (IRepository<>)).To(typeof (Repository<>))
                .WhenAnyAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InNamedScope(ResolutionScopes.UnitOfWork);
            Bind(typeof (IRepository<>)).To(typeof (Repository<>))
                .WhenNoAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InRequestScope();
            Bind(typeof(IRepository<,>)).To(typeof(Repository<,>))
                .WhenAnyAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InNamedScope(ResolutionScopes.UnitOfWork);
            Bind(typeof(IRepository<,>)).To(typeof(Repository<,>))
                .WhenNoAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InRequestScope();

            Bind<IRepositoryFactory>().To<RepositoryFactory>()
                .WhenAnyAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InNamedScope(ResolutionScopes.UnitOfWork);
            Bind<IRepositoryFactory>().To<RepositoryFactory>()
                .WhenNoAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InSingletonScope();

            Bind<IUnitOfWorkFactory>().To<UnitOfWorkFactory>();
            Bind<IUnitOfWork, IUnitOfWorkImplementor>()
                .To<UnitOfWork>()
                .DefinesNamedScope(ResolutionScopes.UnitOfWork);

            //Convention for custom repositories
            Kernel.Bind(o => o
                .From(AppConfiguration.GetDomainAssemblies()
                    .Where(a => a.GetTypes().Any(t => typeof (IRepository).IsAssignableFrom(t))))
                .IncludingNonePublicTypes()
                .Select(t =>
                {
                    if (!t.IsClass || t.IsAbstract || t.IsGenericType || !typeof (IRepository).IsAssignableFrom(t))
                        return false;
                    var repoAttr = t.GetCustomAttribute<RepositoryAttribute>();
                    if (repoAttr != null && !repoAttr.AutoBind) return false;

                    return true;
                })
                .BindAllInterfaces());

            //Events
            Bind<ISaveOrUpdateEventListener>().To<NhSaveOrUpdateEventListener>().InSingletonScope();
            Bind<ISaveOrUpdateEventListener>().To<NhUpdateEventListener>().InSingletonScope();
            Bind<ISaveOrUpdateEventListener>().To<NhSaveEventListener>().InSingletonScope();

            //var interfaces = typeof (ValidatePreInsertUpdateDeleteEventListener).GetInterfaces();
            //Bind(interfaces).To(typeof (ValidatePreInsertUpdateDeleteEventListener)).InSingletonScope();

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
