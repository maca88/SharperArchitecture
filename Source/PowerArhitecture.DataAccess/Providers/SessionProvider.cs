using System;
using System.Web;
using PowerArhitecture.Common.Extensions;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Logging;
using PowerArhitecture.DataAccess.Wrappers;

namespace PowerArhitecture.DataAccess.Providers
{
    public class SessionProvider : IProvider<ISession>
    {
        private readonly ILogger _logger;
        private readonly ISessionFactory _sessionFactory;
        private readonly ISessionManager _sessionManager;

        public SessionProvider(ILogger logger, ISessionFactory sessionFactory, ISessionManager sessionManager)
        {
            _logger = logger;
            _sessionFactory = sessionFactory;
            _sessionManager = sessionManager;
        }

        public object Create(IContext context)
        {
            var session = _sessionFactory.OpenSession();
            Type = session.GetType();

            var sessionInfo = _sessionManager.GetSessionInfo(session);
            if (sessionInfo == null)
                throw new Exception("session is not present in ISessionManager");
            var sessionProps = sessionInfo.SessionProperties;
            sessionProps.SessionResolutionRoot = context.GetContextPreservingResolutionRoot();

            if (context.IsAnyAncestorOrCurrentNamed(ResolutionScopes.UnitOfWork))
            {
                sessionProps.IsManaged = true;
            }
            else if(context.ExistsRequestScope())
            {
                sessionProps.IsManaged = true;
                //TODO: check for transaction attribute, forward as ninject parameter
                session.BeginTransaction();
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
