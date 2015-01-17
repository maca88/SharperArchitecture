using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using NHibernate.Impl;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Wrappers;
using NHibernate;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess.Managers
{
    //This class will rollback all transacions in the current thread when an unhandled exception will occur
    public class SessionManager : ISessionManager, 
        IListener<SessionDisposingEvent>,
        IListener<SessionCreatedEvent>,
        IListener<UnhandledExceptionEvent>
    {

        public class SessionInfo
        {
            public SessionInfo()
            {
                SessionProperties = new SessionProperties();
            }

            public ISession Session { get; set; }

            public Thread Thread { get; set; }

            public SessionWrapper SessionWrapper
            {
                get { return Session as SessionWrapper; }
            }

            public ISession NHibernateSession
            {
                get { return SessionWrapper != null ? SessionWrapper.Session : Session; }
            }

            public SessionProperties SessionProperties { get; set; }
        }


        private readonly ConcurrentSet<SessionInfo> _sessionInfos = new ConcurrentSet<SessionInfo>();

        static readonly object AddLockObj = new object();
        static readonly object RemoveLockObj = new object();

        public SessionManager()
        {
            
        }

        public IEnumerable<ISession> GetAll()
        {
            return GetAll(Thread.CurrentThread);
        }

        public IEnumerable<ISession> GetAll(Thread thread)
        {
            return _sessionInfos.Where(o => o.Thread == thread).Select(o => o.Session);
        }

        public SessionInfo GetSessionInfo(ISession session)
        {
            var sessionWrapper = session as SessionWrapper;
            if (sessionWrapper != null)
                session = sessionWrapper.Session; //Get the Nhibernate Session
            return _sessionInfos.FirstOrDefault(o => ReferenceEquals(o.NHibernateSession, session));
        }

        public void Handle(SessionDisposingEvent e)
        {
            var session = e.Message;
            if (session.IsAlreadyDisposed())
            {
                return;
            }
                
            var sessionInfo = GetSessionInfo(session);
            if (sessionInfo == null)
            {
                throw new Exception("session is not registered");
            }

            DisposeSession(session);

            lock (RemoveLockObj)
            {
                _sessionInfos.Remove(sessionInfo);
            }
        }

        public void Handle(SessionCreatedEvent e)
        {
            var session = e.Message;
            lock (AddLockObj)
            {
                _sessionInfos.Add(new SessionInfo
                {
                    Session = session,
                    Thread = Thread.CurrentThread
                });
            }
            
        }

        public void Handle(UnhandledExceptionEvent message)
        {
            foreach (var session in GetAll())
            {
                RollbackTransaction(session);
            }
        }

        private static void DisposeSession(ISession session)
        {
            try
            {
                //session.Flush(); -- Creates extra versions
                CommitTransaction(session);
            }
            catch (Exception)
            {
                RollbackTransaction(session);
                throw;
            }
            finally
            {
                if(session.IsOpen) //Can be closed by others (i.e. Breeze)
                    session.Close(); //Dispose will happen in SessionWrapper
            } 
        }

        private static void RollbackTransaction(ISession session)
        {
            if (session.Transaction == null || session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return;
            session.Transaction.Rollback();
            session.Transaction.Dispose();
        }

        private static void CommitTransaction(ISession session)
        {
            if (session.Transaction == null || session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return;
            session.Transaction.Commit();
            session.Transaction.Dispose();
        }
    }
}
