using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Event;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Decorators;
using PowerArhitecture.DataAccess.EventListeners;
using PowerArhitecture.DataAccess.Events;
using SimpleInjector;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.DataAccess.Providers
{
    internal class SessionProvider : ISessionProvider
    {
        private readonly ILogger _logger;
        private readonly Container _container;
        private readonly IEventPublisher _eventPublisher;
        internal static readonly ConcurrentSet<Guid> RegisteredSessionIds = new ConcurrentSet<Guid>();

        public SessionProvider(ILogger logger, Container container, IEventPublisher eventPublisher)
        {
            _logger = logger;
            _container = container;
            _eventPublisher = eventPublisher;
        }

        public ISession Create(string name)
        {
            var sessionFactory = _container.GetDatabaseService<ISessionFactory>(name);
            return new SessionDecorator(new Lazy<IEventSource>(() =>
            {
                var eventSource = (IEventSource) sessionFactory.OpenSession();
                RegisteredSessionIds.Add(eventSource.SessionId);
                eventSource.Transaction.RegisterSynchronization(new TransactionEventListener(eventSource.Unwrap(), _eventPublisher));
                _eventPublisher.Publish(new SessionCreatedEvent(eventSource, name));
                return eventSource;
            }), _eventPublisher, name);
        }

        public ISession Get(string dbConfigName = null)
        {
            dbConfigName = dbConfigName ?? DatabaseConfiguration.DefaultName;
            return _container.GetDatabaseService<ISession>(dbConfigName);
        }
    }
}
