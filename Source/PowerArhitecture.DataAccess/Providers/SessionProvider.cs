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
        private readonly IEventAggregator _eventAggregator;

        public SessionProvider(ILogger logger, ISessionFactory defaultSessionFactory, IResolutionRoot resolutionRoot, 
            IEventAggregator eventAggregator)
        {
            _logger = logger;
            _defaultSessionFactory = defaultSessionFactory;
            _resolutionRoot = resolutionRoot;
            _eventAggregator = eventAggregator;
        }

        public object Create(IContext context)
        {
            var sessionFactory = _defaultSessionFactory;
            string sfName = null;
            if (context.Request.Target != null)
            {
                var attr = (NamedSessionFactoryAttribute)context.Request.Target.GetCustomAttributes(typeof(NamedSessionFactoryAttribute), true).FirstOrDefault();
                sfName = attr?.Name;
            }
            
            //If there is no NamedSessionFactoryAttribute check if there is NamedSessionFactoryParameter
            if (sfName == null)
            {
                var param = context.Request.Parameters.OfType<NamedSessionFactoryParameter>().FirstOrDefault();
                sfName = param?.Name;
            }

            if (sfName != null)
            {
                sessionFactory = _resolutionRoot.Get<ISessionFactory>(new NamedSessionFactoryParameter(sfName));
            }
            var sessionContext = new SessionContext
            {
                IsManaged = true,
                ResolutionRoot = context.GetContextPreservingResolutionRoot(),
                CurrentCultureInfo = Thread.CurrentThread.CurrentCulture
            };
            var eventSource = (IEventSource) sessionFactory.OpenSession(sessionContext);
            var session = (ISession)new SessionWrapper(eventSource, _eventAggregator);
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
                session.Transaction.RegisterSynchronization(new TransactionEventListener(eventSource, _eventAggregator));
            }
            else //If a session is created when HttpContext is not available tell SessionManager that the session must be manually disposed
            {
                _logger.Warn("An unmanaged session was created");
                //_sessionManager.MarkAsUnmanaged(session);
            }
            return session;
        }

        public Type Type { get; private set; }
        
    }
}
