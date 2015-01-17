using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using PowerArhitecture.Common.Extensions;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.EventListeners;
using PowerArhitecture.DataAccess.Factories;
using PowerArhitecture.DataAccess.Managers;
using PowerArhitecture.DataAccess.NHEventListeners;
using PowerArhitecture.DataAccess.Providers;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Factory;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using Ninject.Web.Common;

namespace PowerArhitecture.DataAccess
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<Configuration>().ToSelf().InSingletonScope();
            Bind<ISessionFactory>().ToProvider<SessionFactoryProvider>()
                .InSingletonScope()
                .OnActivation(SessionFactoryProvider.PopulateData);
            //Session - Activation and deactivation is now handled by session manager
            Bind<ISession>()
                .ToProvider<SessionProvider>()
                .WhenRequestScopeExistsAndNoAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InRequestScope();
            Bind<ISession>()
                .ToProvider<SessionProvider>()
                .WhenRequestScopeNotExistsAndNoAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InCallScope();
            Bind<ISession>()
                .ToProvider<SessionProvider>()
                .WhenAnyAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InNamedScope(ResolutionScopes.UnitOfWork);

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

            //.ToFactory(); ninject factory are not good for generic -> manual registration for each type
            Bind<IRepositoryFactory>().To<RepositoryFactory>()
                .WhenAnyAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InNamedScope(ResolutionScopes.UnitOfWork);
            Bind<IRepositoryFactory>().To<RepositoryFactory>()
                .WhenNoAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork)
                .InSingletonScope();

            Bind<IUnitOfWorkFactory>().ToFactory();
            Bind<IUnitOfWork>()
                .To<UnitOfWork>()
                .NamedLikeFactoryMethod<UnitOfWork, IUnitOfWorkFactory>(f => f.GetNew(IsolationLevel.Unspecified))
                .DefinesNamedScope(ResolutionScopes.UnitOfWork);

            //Convention for custom repositories
            Kernel.Bind(o => o
                .From(AppDomain.CurrentDomain.GetAssemblies()
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

            /* Will be bind by convention of listeners
            Bind<ISessionManager, SessionManager>().To<SessionManager>().InSingletonScope();
            Bind<ISessionEventListener, SessionEventListener>().To<SessionEventListener>().InSingletonScope();
            //Bind<ISharedEngineProvider>().To<NHibernateSharedEngineProvider>().InSingletonScope();
            */

            //Events
            Bind<ISaveOrUpdateEventListener>().To<NhSaveOrUpdateEventListener>().InSingletonScope();
            Bind<ISaveOrUpdateEventListener>().To<NhUpdateEventListener>().InSingletonScope();
            Bind<ISaveOrUpdateEventListener>().To<NhSaveEventListener>().InSingletonScope();
            Bind<IPreCollectionUpdateEventListener, IPreInsertEventListener, IPreUpdateEventListener, IPreDeleteEventListener>()
                .To<ValidatePreInsertUpdateDeleteEventListener>()
                .InSingletonScope();
            Bind<IPreUpdateEventListener, IPreInsertEventListener>()
                .To<NhPreInsertUpdateEventListener>()
                .InSingletonScope();
            Bind<IPreUpdateEventListener, IPreInsertEventListener>()
                .To<RootAggregatePreUpdateInsertEventListener>()
                .InSingletonScope();
        }
    }
}
