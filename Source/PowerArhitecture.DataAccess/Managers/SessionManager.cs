using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
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
        private readonly ConcurrentDictionary<Thread, ConcurrentDictionary<ISession, SessionProperties>> _sessions = 
            new ConcurrentDictionary<Thread, ConcurrentDictionary<ISession, SessionProperties>>();
        static readonly object AddLockObj = new object();
        static readonly object RemoveLockObj = new object();

        public IEnumerable<ISession> GetAll()
        {
            return GetAll(Thread.CurrentThread);
        }

        public SessionProperties GetSessionProperties(ISession session)
        {
            if (!_sessions.ContainsKey(Thread.CurrentThread) || !_sessions[Thread.CurrentThread].ContainsKey(session))
                throw new Exception("Session is not present in the SessionManager");
            return _sessions[Thread.CurrentThread][session];
        }

        public IEnumerable<ISession> GetAll(Thread thread)
        {
            var result = new ConcurrentSet<ISession>();
            return !_sessions.ContainsKey(thread) ? result : _sessions[thread].Keys;
        }

        public void Handle(SessionDisposingEvent e)
        {
            var session = e.Message;
            ConcurrentDictionary<ISession, SessionProperties> dict = null;
            Thread thread = null;
            if (!_sessions.ContainsKey(Thread.CurrentThread) || !_sessions[Thread.CurrentThread].ContainsKey(session))
            {
                //TODO: there where done some changes about disposing in WebApi2, investigate it
                foreach (var pair in _sessions.Where(pair => pair.Value.ContainsKey(session)))
                {
                    thread = pair.Key;
                    dict = pair.Value;
                    break;
                }
                if (dict == null)
                    return;
            }
            else
            {
                dict = _sessions[Thread.CurrentThread];
                thread = Thread.CurrentThread;
            }
            DisposeSession(session);

            lock (RemoveLockObj)
            {
                SessionProperties props;
                dict.TryRemove(session, out props);
            }
            if (dict.Any()) return;

            _sessions.TryRemove(thread, out dict);
        }

        public void Handle(SessionCreatedEvent e)
        {
            var session = e.Message;
            if (!_sessions.ContainsKey(Thread.CurrentThread))
                _sessions.TryAdd(Thread.CurrentThread, new ConcurrentDictionary<ISession, SessionProperties>());

            lock (AddLockObj)
            {
                _sessions[Thread.CurrentThread].TryAdd(session, new SessionProperties());
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
