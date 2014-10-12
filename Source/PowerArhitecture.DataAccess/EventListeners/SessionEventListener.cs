using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Wrappers;
using NHibernate;

namespace PowerArhitecture.DataAccess.EventListeners
{
    public class SessionEventListener : ISessionEventListener,
        IListener<TransactionCommittedEvent>,
        IListener<TransactionCommittingEvent>
    {
        private readonly ConcurrentDictionary<SessionListenerType, ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>>> _dict = 
            new ConcurrentDictionary<SessionListenerType, ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>>>();

        public void Handle(TransactionCommittedEvent e)
        {
            HandleEvent(SessionListenerType.AfterCommit, e.Message);
        }

        public void Handle(TransactionCommittingEvent e)
        {
            HandleEvent(SessionListenerType.BeforeCommit, e.Message);
        }

        public void AddAListener(SessionListenerType type, ISession session, Action action)
        {
            AddAListener(type, session, s => action());
        }

        public void AddAListener(SessionListenerType type, ISession session, Action<ISession> action)
        {
            if (!_dict.ContainsKey(type))
                _dict.GetOrAdd(type, o => new ConcurrentDictionary<ISession, SynchronizedCollection<Action<ISession>>>());

            if (!_dict[type].ContainsKey(session))
                _dict[type].GetOrAdd(session, o => new SynchronizedCollection<Action<ISession>>());
            _dict[type][session].Add(action);
        }

        private void HandleEvent(SessionListenerType type, ISession session)
        {
            if (!_dict.ContainsKey(type) || !_dict[type].ContainsKey(session))
            {
                var wrapper = session as SessionWrapper;
                if(wrapper != null)
                    HandleEvent(type, wrapper.Session);
                return;
            }

            foreach (var action in _dict[type][session])
            {
                action(session);
            }
            _dict[type][session].Clear();
            SynchronizedCollection<Action<ISession>> ss;
            if(!_dict[type].TryRemove(session, out ss))
                throw new Exception("TryRemove session failed");
        }
    }
}
