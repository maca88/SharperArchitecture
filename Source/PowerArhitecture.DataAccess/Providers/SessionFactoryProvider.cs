using System;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Wrappers;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using NHibernate.Cfg;
using Ninject;
using Ninject.Activation;

namespace PowerArhitecture.DataAccess.Providers
{
    public class SessionFactoryProvider : IProvider<ISessionFactory>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDatabaseSettings _dalSettings;
        private readonly Configuration _configuration;

        public SessionFactoryProvider(IEventAggregator eventAggregator, IDatabaseSettings dalSettings, Configuration configuration)
        {
            _eventAggregator = eventAggregator;
            _configuration = configuration;
            _dalSettings = dalSettings;
        }

        public object Create(IContext context)
        {
            var sessionFactory = new SessionFactoryWrapper(Database.CreateSessionFactory(_configuration,
                _eventAggregator, _dalSettings), _eventAggregator);
            Type = sessionFactory.GetType();
            return sessionFactory;
        }

        internal static void PopulateData(IContext context, ISessionFactory sessionFactory)
        {
            var settings = context.Kernel.Get<IDatabaseSettings>();
            var eventAggregator = context.Kernel.Get<IEventAggregator>();
            
            if (!settings.RecreateAtStartup) return;
            using (var unitOfWork = context.Kernel.Get<IUnitOfWork>())
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

        public System.Type Type { get; private set; }
    }
}
