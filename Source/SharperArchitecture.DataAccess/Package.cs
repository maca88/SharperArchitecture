using System;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Event;
using SharperArchitecture.Common.Configuration;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.EventListeners;
using SharperArchitecture.DataAccess.Events;
using SharperArchitecture.DataAccess.Factories;
using SharperArchitecture.DataAccess.Internal;
using SharperArchitecture.DataAccess.NHEventListeners;
using SharperArchitecture.DataAccess.Providers;
using SharperArchitecture.DataAccess.Specifications;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Packaging;

namespace SharperArchitecture.DataAccess
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

            container.Register<IDbStore, DbStore>(Lifestyle.Scoped);
            container.RegisterSingleton<IQueryProcessor, DefaultQueryProcessor>();

            var depAssemblies = Assembly.GetExecutingAssembly()
                .GetDependentAssemblies()
                .ToList();
            container.Register(typeof(IQueryHandler<,>), depAssemblies, Lifestyle.Scoped);
            container.Register(typeof(IAsyncQueryHandler<,>), depAssemblies, Lifestyle.Scoped);

            // Registration for the default database session
            registration = Lifestyle.Scoped.CreateRegistration(() =>
            {
                return container.GetInstance<SessionProvider>().Create(DatabaseConfiguration.DefaultName);
            }, container);
            container.AddRegistration(typeof(ISession), registration);

            // Initializer for the session factory to trigger the data population if the database recreation is set
            container.RegisterInitializer<ISessionFactory>(sessionFactory =>
            {
                var info = Database.GetSessionFactoryInfo(sessionFactory);
                if (!info.DatabaseConfiguration.RecreateAtStartup)
                {
                    return;
                }
                var eventPublisher = container.GetInstance<IEventPublisher>();
                using (var session = sessionFactory.OpenSession())
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        eventPublisher.Publish(new PopulateDbEvent(session));
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            });
        }
    }
}
