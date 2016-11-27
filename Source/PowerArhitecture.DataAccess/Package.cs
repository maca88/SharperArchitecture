using System;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Event;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.SimpleInjector;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Decorators;
using PowerArhitecture.DataAccess.EventListeners;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.DataAccess.Factories;
using PowerArhitecture.DataAccess.NHEventListeners;
using PowerArhitecture.DataAccess.Providers;
using PowerArhitecture.DataAccess.Specifications;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Diagnostics;
using SimpleInjector.Extensions;
using SimpleInjector.Packaging;

namespace PowerArhitecture.DataAccess
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

            var registration = Lifestyle.Singleton.CreateRegistration<SessionFactoryProvider>(container);
            container.AddRegistration(typeof(ISessionFactoryProvider), registration);
            container.AddRegistration(typeof(SessionFactoryProvider), registration);

            registration = Lifestyle.Singleton.CreateRegistration<SessionProvider>(container);
            container.AddRegistration(typeof(ISessionProvider), registration);
            container.AddRegistration(typeof(SessionProvider), registration);

            registration = Lifestyle.Transient.CreateRegistration<UnitOfWork>(container);
            container.AddRegistration(typeof(UnitOfWork), registration);
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                    "UnitOfWork wraps an execution context scope and must be manually disposed");
            // Simple Injector does not support injection of primitive values so the workaround is to set it manually after the creation
            container.RegisterWithContext<IUnitOfWork>(ctx =>
            {
                var attr = ctx.Parameter?.GetCustomAttribute<IsolationLevelAttribute>();
                var instance = container.GetInstance<UnitOfWork>();
                if (attr != null)
                {
                    instance.IsolationLevel = attr.Level;
                }
                return instance;
            }, r =>
            {
                r.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                    "UnitOfWork wraps an execution context scope and must be manually disposed");
            });

            // Events
            registration = Lifestyle.Singleton.CreateRegistration<NhSaveOrUpdateEventListener>(container);
            container.AppendToCollection(typeof(ISaveOrUpdateEventListener), registration);
            registration = Lifestyle.Singleton.CreateRegistration<NhUpdateEventListener>(container);
            container.AppendToCollection(typeof(ISaveOrUpdateEventListener), registration);
            registration = Lifestyle.Singleton.CreateRegistration<NhSaveEventListener>(container);
            container.AppendToCollection(typeof(ISaveOrUpdateEventListener), registration);

            registration = Lifestyle.Singleton.CreateRegistration<EntityPreUpdateInsertEventListener>(container);
            container.AppendToCollection(typeof(IPreInsertEventListener), registration);
            container.AppendToCollection(typeof(IPreUpdateEventListener), registration);

            // Register a empty collection for each NHibernate event so that NhConfigurationEventHandler will work
            container.RegisterCollection<IFlushEventListener>();
            container.RegisterCollection<IDeleteEventListener>();
            container.RegisterCollection<IAutoFlushEventListener>();
            container.RegisterCollection<IPreDeleteEventListener>();
            container.RegisterCollection<IPostInsertEventListener>();
            container.RegisterCollection<IPostUpdateEventListener>();
            container.RegisterCollection<IPostCollectionUpdateEventListener>();
            container.RegisterCollection<IPostDeleteEventListener>();

            registration = Lifestyle.Singleton.CreateRegistration<AuditEntityEventListener>(container);
            container.AppendToCollection(typeof(IPreUpdateEventListener), registration);

            container.RegisterSingleton<IAuditUserProvider, AuditUserProvider>();

            // Default session must be registered at the end
            registration = Lifestyle.Scoped.CreateRegistration(() =>
            {
                return container.GetInstance<SessionProvider>().Create(DatabaseConfiguration.DefaultName);
            }, container);
            container.RegisterConditional(typeof(ISession), registration, SimpleInjectorExtensions.NotHandledPredicate);

            // Initializer for the session factory to trigger the data population if the database recreation is set
            container.RegisterInitializer<ISessionFactory>(sessionFactory =>
            {
                var info = Database.GetSessionFactoryInfo(sessionFactory);
                if (!info.DatabaseConfiguration.RecreateAtStartup)
                {
                    return;
                }
                var eventPublisher = container.GetInstance<IEventPublisher>();
                using (var unitOfWork = container.GetInstance<IUnitOfWork>())
                {
                    try
                    {
                        eventPublisher.Publish(new PopulateDbEvent(unitOfWork));
                        unitOfWork.Commit();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }
            });
        }
    }
}
