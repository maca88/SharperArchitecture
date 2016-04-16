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
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Parameters;

namespace PowerArhitecture.DataAccess.Providers
{
    public class SessionFactoryProvider : IProvider<ISessionFactory>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IResolutionRoot _resolutionRoot;
        private readonly static Dictionary<string, ISessionFactory> Factories = new Dictionary<string, ISessionFactory>();
        private const string DefaultName = "Default";

        public SessionFactoryProvider(IEventAggregator eventAggregator, IResolutionRoot resolutionRoot)
        {
            _eventAggregator = eventAggregator;
            _resolutionRoot = resolutionRoot;
        }

        public object Create(IContext context)
        {
            string name = null;
            if (context.Request.Target != null)
            {
                var attr = (NamedSessionFactoryAttribute)context.Request.Target.GetCustomAttributes(typeof(NamedSessionFactoryAttribute), true).FirstOrDefault();
                name = attr?.Name;
            }
           
            //If there is no NamedSessionFactoryAttribute check if there is NamedSessionFactoryParameter
            if (name == null)
            {
                var param = context.Request.Parameters.OfType<NamedSessionFactoryParameter>().FirstOrDefault();
                name = param?.Name;
            }

            DatabaseConfiguration dbConfiguration;

            if (name != null)
            {
                dbConfiguration = _resolutionRoot.TryGet<DatabaseConfiguration>(new NamedSessionFactoryParameter(name));
                if(dbConfiguration == null)
                    throw new HibernateConfigException($"IDatabaseConfiguration is not registered for named session factory '{name}'");
            }
            else
            {
                dbConfiguration = _resolutionRoot.Get<DatabaseConfiguration>();
            }

            var sessionFactory = Database.CreateSessionFactory(_eventAggregator, dbConfiguration, name);

            Factories[name ?? DefaultName] = sessionFactory;

            Type = sessionFactory.GetType();
            return sessionFactory;
        }

        internal static void PopulateData(IContext context, ISessionFactory sessionFactory)
        {
            var eventAggregator = context.Kernel.Get<IEventAggregator>();

            var pair = Factories.FirstOrDefault(o => ReferenceEquals(o.Value, sessionFactory));

            if (pair.Value == null)
                throw new PowerArhitectureException(
                    "Database population is not supported when SessionFactory is created manualy");
            var name = pair.Key == DefaultName ? null : pair.Key;
            var settings = string.IsNullOrEmpty(name)
                ? context.Kernel.Get<DatabaseConfiguration>()
                : context.Kernel.Get<DatabaseConfiguration>(new NamedSessionFactoryParameter(name));

            if (!settings.RecreateAtStartup) return;
            using (var unitOfWork = context.Kernel.Get<IUnitOfWorkFactory>().GetNew(IsolationLevel.Unspecified, name))
            {
                try
                {
                    eventAggregator.SendMessage(new PopulateDbEvent(unitOfWork));
                }
                catch (Exception ex)
                {
                    eventAggregator.SendMessage(new UnhandledExceptionEvent(ex));
                    throw;
                }
            }
        }

        public Type Type { get; private set; }
    }
}
