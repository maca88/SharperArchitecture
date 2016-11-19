using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.DataAccess.Specifications;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace PowerArhitecture.DataAccess.Providers
{
    internal class SessionFactoryProvider
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Container _container;

        public SessionFactoryProvider(IEventPublisher eventPublisher, Container container)
        {
            _eventPublisher = eventPublisher;
            _container = container;
        }

        public ISessionFactory Create(string dbConfigName)
        {
            var dbConfiguration = _container.GetDatabaseService<DatabaseConfiguration>(dbConfigName);
            var sessionFactory = Database.CreateSessionFactory(_eventPublisher, dbConfiguration);
            _eventPublisher.Publish(new SessionFactoryCreatedEvent(sessionFactory));
            return sessionFactory;
        }
    }
}
