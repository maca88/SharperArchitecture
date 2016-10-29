using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using Ninject.Extensions.Logging;

namespace PowerArhitecture.DataAccess.EventListeners
{
    public class SessionEventProvider : ISessionEventProvider,
        IListener<TransactionCommittedEvent>,
        IListener<TransactionCommittingEvent>
    {
        private readonly ConcurrentDictionary<SessionListenerType, ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>>> _dict = 
            new ConcurrentDictionary<SessionListenerType, ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>>>();

        private readonly ILogger _logger;

        public SessionEventProvider(ILogger logger)
        {
            _logger = logger;
        }

        public void Handle(TransactionCommittedEvent e)
        {
            HandleEvent(SessionListenerType.AfterCommit, e.Message);
        }

        public Task HandleAsync(TransactionCommittedEvent e)
        {
            HandleEvent(SessionListenerType.AfterCommit, e.Message);
            return Task.CompletedTask;
        }

        public void Handle(TransactionCommittingEvent e)
        {
            HandleEvent(SessionListenerType.BeforeCommit, e.Message);
        }

        public Task HandleAsync(TransactionCommittingEvent e)
        {
            HandleEvent(SessionListenerType.BeforeCommit, e.Message);
            return Task.CompletedTask;
        }

        public void AddListener(SessionListenerType type, ISession session, Action action)
        {
            AddListener(type, session, s => action());
        }

        public void AddListener(SessionListenerType type, ISession session, Action<ISession> action)
        {
            session = session.Unwrap();
            var typeDict = _dict.GetOrAdd(type, o => new ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>>());
            var syncCol = typeDict.GetOrAdd(session, o => new SynchronizedCollection<Action<ISession>>());
            syncCol.Add(action);
        }

        private void HandleEvent(SessionListenerType type, ISession session)
        {
            session = session.Unwrap();
            ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>> typeDict;
            SynchronizedCollection<Action<ISession>> syncCol;
            if (!_dict.TryGetValue(type, out typeDict) || !typeDict.TryGetValue(session, out syncCol))
            {
                return;
            }

            foreach (var action in syncCol)
            {
                action(session);
            }

            syncCol.Clear();
            SynchronizedCollection<Action<ISession>> ss;
            if (!typeDict.TryRemove(session, out ss))
            {
                throw new Exception("TryRemove session failed");
            }
        }

    }
}
