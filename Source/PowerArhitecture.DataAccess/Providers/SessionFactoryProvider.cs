using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using FluentNHibernate.Cfg;
using Ninject.Syntax;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Wrappers;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using NHibernate.Cfg;
using Ninject;
using Ninject.Activation;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Parameters;

namespace PowerArhitecture.DataAccess.Providers
{
    public class SessionFactoryProvider : IProvider<ISessionFactory>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IResolutionRoot _resolutionRoot;
        private static readonly Dictionary<string, ISessionFactory> Factories = new Dictionary<string, ISessionFactory>();

        public SessionFactoryProvider(IEventPublisher eventPublisher, IResolutionRoot resolutionRoot)
        {
            _eventPublisher = eventPublisher;
            _resolutionRoot = resolutionRoot;
        }

        public object Create(IContext context)
        {
            string name = null;
            if (context.Request.Target != null)
            {
                var attr = (DatabaseAttribute)context.Request.Target.GetCustomAttributes(typeof(DatabaseAttribute), true).FirstOrDefault();
                name = attr?.ConfigurationName;
            }

            //If there is no DatabaseAttribute check if there is DatabaseConfigurationParameter
            if (name == null)
            {
                var param = context.Request.Parameters.OfType<DatabaseConfigurationParameter>().FirstOrDefault();
                name = param?.Name;
            }

            DatabaseConfiguration dbConfiguration;

            if (name != null)
            {
                dbConfiguration = _resolutionRoot.TryGet<DatabaseConfiguration>(new DatabaseConfigurationParameter(name));
                if(dbConfiguration == null)
                    throw new HibernateConfigException($"DatabaseConfiguration is not registered for name '{name}'");
            }
            else
            {
                dbConfiguration = _resolutionRoot.Get<DatabaseConfiguration>();
            }

            var sessionFactory = Database.CreateSessionFactory(_eventPublisher, dbConfiguration);
            _eventPublisher.Publish(new SessionFactoryCreatedEvent(sessionFactory));
            Factories[dbConfiguration.Name] = sessionFactory;

            Type = sessionFactory.GetType();
            return sessionFactory;
        }

        internal static void PopulateData(IContext context, ISessionFactory sessionFactory)
        {
            var eventPublisher = context.Kernel.Get<IEventPublisher>();

            var pair = Factories.FirstOrDefault(o => ReferenceEquals(o.Value, sessionFactory));

            if (pair.Value == null)
                throw new PowerArhitectureException(
                    "Database population is not supported when SessionFactory is created manualy");
            var name = pair.Key;
            var settings = context.Kernel.Get<DatabaseConfiguration>(new DatabaseConfigurationParameter(name));

            if (!settings.RecreateAtStartup)
            {
                return;
            }
            using (var unitOfWork = context.Kernel.Get<IUnitOfWorkFactory>().GetNew(IsolationLevel.Unspecified, name))
            {
                try
                {
                    eventPublisher.Publish(new PopulateDbEvent(unitOfWork));
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    eventPublisher.Publish(new UnhandledExceptionEvent(ex));
                    throw;
                }
            }
        }

        public Type Type { get; private set; }
    }
}
