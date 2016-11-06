using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using Ninject.Syntax;
using PowerArhitecture.Common.Extensions;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using NHibernate.Event;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Logging;
using Ninject.Planning.Bindings;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.EventListeners;
using PowerArhitecture.DataAccess.Parameters;
using PowerArhitecture.DataAccess.Wrappers;

namespace PowerArhitecture.DataAccess.Providers
{
    public class SessionProvider : IProvider<ISession>
    {
        private readonly ILogger _logger;
        private readonly ISessionFactory _defaultSessionFactory;
        private readonly IResolutionRoot _resolutionRoot;
        private readonly IEventPublisher _eventPublisher;

        public SessionProvider(ILogger logger, ISessionFactory defaultSessionFactory, IResolutionRoot resolutionRoot, 
            IEventPublisher eventPublisher)
        {
            _logger = logger;
            _defaultSessionFactory = defaultSessionFactory;
            _resolutionRoot = resolutionRoot;
            _eventPublisher = eventPublisher;
        }

        public object Create(IContext context)
        {
            var sessionFactory = _defaultSessionFactory;
            string sfName = null;
            if (context.Request.Target != null)
            {
                var attr = (DatabaseAttribute)context.Request.Target.GetCustomAttributes(typeof(DatabaseAttribute), true).FirstOrDefault();
                sfName = attr?.ConfigurationName;
            }

            //If there is no DatabaseAttribute check if there is DatabaseConfigurationParameter
            if (sfName == null)
            {
                var param = context.Request.Parameters.OfType<DatabaseConfigurationParameter>().FirstOrDefault();
                sfName = param?.Name;
            }

            if (sfName != null)
            {
                sessionFactory = _resolutionRoot.Get<ISessionFactory>(new DatabaseConfigurationParameter(sfName));
            }
            var sessionContext = new SessionContext
            {
                ResolutionRoot = context.GetContextPreservingResolutionRoot(),
                CurrentCultureInfo = Thread.CurrentThread.CurrentCulture
            };
            var eventSource = (IEventSource) sessionFactory.OpenSession(sessionContext);
            var session = (ISession)new SessionWrapper(eventSource, _eventPublisher);
            Type = session.GetType();
            session.FlushMode = FlushMode.Commit; //HACK ... TODO: update to 4.1

            if (context.IsAnyAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork))
            {
                sessionContext.IsManaged = true;
            }
            else if(context.ExistsRequestScope())
            {
                sessionContext.IsManaged = true;
                //TODO: check for transaction attribute, forward as ninject parameter
                session.BeginTransaction();
                session.Transaction.RegisterSynchronization(new TransactionEventListener(eventSource, _eventPublisher));
            }
            else // A session is created when HttpContext is not available
            {
                _logger.Warn("An unmanaged session was created");
            }
            return session;
        }

        public Type Type { get; private set; }
        
    }
}
