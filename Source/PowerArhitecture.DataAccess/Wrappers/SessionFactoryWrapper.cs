using System;
using System.Collections.Generic;
using System.Data;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Stat;

namespace PowerArhitecture.DataAccess.Wrappers
{
    //User for dispatching session events
    public class SessionFactoryWrapper : ISessionFactory
    {
        private readonly IEventAggregator _eventAggregator;

        public SessionFactoryWrapper(ISessionFactory sessionFactory, IEventAggregator eventAggregator)
        {
            SessionFactory = sessionFactory;
            _eventAggregator = eventAggregator;
            _eventAggregator.SendMessage(new SessionFactoryInitializedEvent(sessionFactory));
        }


        public ISessionFactory SessionFactory { get; private set; }
        public void Dispose()
        {
            SessionFactory.Dispose();
        }

        public ISession OpenSession(IDbConnection conn)
        {
            var session = new SessionWrapper(SessionFactory.OpenSession(conn), _eventAggregator);
            _eventAggregator.SendMessage(new SessionCreatedEvent(session));
            return session;
        }

        public ISession OpenSession(IInterceptor sessionLocalInterceptor)
        {
            var session = new SessionWrapper(SessionFactory.OpenSession(sessionLocalInterceptor), _eventAggregator);
            _eventAggregator.SendMessage(new SessionCreatedEvent(session));
            return session;
        }

        public ISession OpenSession(IDbConnection conn, IInterceptor sessionLocalInterceptor)
        {
            var session = new SessionWrapper(SessionFactory.OpenSession(conn, sessionLocalInterceptor), _eventAggregator);
            _eventAggregator.SendMessage(new SessionCreatedEvent(session));
            return session;
        }

        public ISession OpenSession()
        {
            var session = new SessionWrapper(SessionFactory.OpenSession(), _eventAggregator);
            _eventAggregator.SendMessage(new SessionCreatedEvent(session));
            return session;
        }

        public IClassMetadata GetClassMetadata(Type persistentClass)
        {
            return SessionFactory.GetClassMetadata(persistentClass);
        }

        public IClassMetadata GetClassMetadata(string entityName)
        {
            return SessionFactory.GetClassMetadata(entityName);
        }

        public ICollectionMetadata GetCollectionMetadata(string roleName)
        {
            return SessionFactory.GetCollectionMetadata(roleName);
        }

        public IDictionary<string, IClassMetadata> GetAllClassMetadata()
        {
            return SessionFactory.GetAllClassMetadata();
        }

        public IDictionary<string, ICollectionMetadata> GetAllCollectionMetadata()
        {
            return SessionFactory.GetAllCollectionMetadata();
        }

        public void Close()
        {
            SessionFactory.Close();
        }

        public void Evict(Type persistentClass)
        {
            SessionFactory.Evict(persistentClass);
        }

        public void Evict(Type persistentClass, object id)
        {
            SessionFactory.Evict(persistentClass, id);
        }

        public void EvictEntity(string entityName)
        {
            SessionFactory.EvictEntity(entityName);
        }

        public void EvictEntity(string entityName, object id)
        {
            SessionFactory.EvictEntity(entityName, id);
        }

        public void EvictCollection(string roleName)
        {
            SessionFactory.EvictCollection(roleName);
        }

        public void EvictCollection(string roleName, object id)
        {
            SessionFactory.EvictCollection(roleName, id);
        }

        public void EvictQueries()
        {
            SessionFactory.EvictQueries();
        }

        public void EvictQueries(string cacheRegion)
        {
            SessionFactory.EvictQueries(cacheRegion);
        }

        public IStatelessSession OpenStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }

        public IStatelessSession OpenStatelessSession(IDbConnection connection)
        {
            return SessionFactory.OpenStatelessSession(connection);
        }

        public FilterDefinition GetFilterDefinition(string filterName)
        {
            return SessionFactory.GetFilterDefinition(filterName);
        }

        public ISession GetCurrentSession()
        {
            return SessionFactory.GetCurrentSession();
        }

        public IStatistics Statistics { get { return SessionFactory.Statistics; } }
        public bool IsClosed { get { return SessionFactory.IsClosed; } }
        public ICollection<string> DefinedFilterNames { get { return SessionFactory.DefinedFilterNames; } }
    }
}
