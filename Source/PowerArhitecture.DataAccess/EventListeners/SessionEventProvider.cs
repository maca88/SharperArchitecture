using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using Ninject.Extensions.Logging;

namespace PowerArhitecture.DataAccess.EventListeners
{
    public class SessionSubscriptionManager : BaseEventsHandler<TransactionCommittedEvent, TransactionCommittingEvent>, ISessionSubscriptionManager
    {
        private readonly ConcurrentDictionary<SessionSubscription, ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>>> _dict = 
            new ConcurrentDictionary<SessionSubscription, ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>>>();

        private readonly ILogger _logger;

        public SessionSubscriptionManager(ILogger logger)
        {
            _logger = logger;
        }

        public override void Handle(TransactionCommittedEvent e)
        {
            HandleEvent(SessionSubscription.AfterCommit, e.Message);
        }

        public override Task HandleAsync(TransactionCommittedEvent e, CancellationToken cancellationToken)
        {
            HandleEvent(SessionSubscription.AfterCommit, e.Message);
            return Task.CompletedTask;
        }

        public override void Handle(TransactionCommittingEvent e)
        {
            HandleEvent(SessionSubscription.BeforeCommit, e.Message);
        }

        public override Task HandleAsync(TransactionCommittingEvent e, CancellationToken cancellationToken)
        {
            HandleEvent(SessionSubscription.BeforeCommit, e.Message);
            return Task.CompletedTask;
        }

        public void Subscribe(SessionSubscription type, ISession session, Action action)
        {
            Subscribe(type, session, s => action());
        }

        public void Subscribe(SessionSubscription type, ISession session, Action<ISession> action)
        {
            session = session.Unwrap();
            var typeDict = _dict.GetOrAdd(type, o => new ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>>());
            var syncCol = typeDict.GetOrAdd(session, o => new SynchronizedCollection<Action<ISession>>());
            syncCol.Add(action);
        }

        private void HandleEvent(SessionSubscription type, ISession session)
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
