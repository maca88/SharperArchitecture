using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Engine;
using NHibernate.Impl;
using Ninject.Extensions.Logging;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Wrappers;
using NHibernate;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess.Managers
{
    public class SessionManager : ISessionManager
    {
        private readonly ConcurrentSet<SessionInfo> _sessionInfos = new ConcurrentSet<SessionInfo>();
        private readonly ILogger _logger;

        public class SessionInfo
        {
            public SessionInfo(ISession session)
            {
                SessionProperties = new SessionProperties();
                Session = session;
            }

            public ISession Session { get; }

            public SessionProperties SessionProperties { get; set; }
        }

        public SessionManager(ILogger logger)
        {
            _logger = logger;
        }

        public SessionInfo Add(ISession session)
        {
            var info = new SessionInfo(session);
            _sessionInfos.Add(info);
            return info;
        }

        public bool Remove(ISession session)
        {
            var sessionInfo = GetSessionInfo(session);
            if (sessionInfo == null)
            {
                return false;
            }
            _sessionInfos.Remove(sessionInfo);
            return true;
        }

        public IEnumerable<ISession> GetAll()
        {
            return _sessionInfos.Select(o => o.Session);
        }

        public SessionInfo GetSessionInfo(ISession session)
        {
            return _sessionInfos.FirstOrDefault(o => ReferenceEquals(o.Session, session));
        }
    }
}
