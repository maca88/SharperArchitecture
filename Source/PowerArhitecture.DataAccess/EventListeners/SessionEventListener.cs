using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Enums;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Wrappers;
using NHibernate;

namespace PowerArhitecture.DataAccess.EventListeners
{
    public class SessionEventProvider : ISessionEventProvider,
        IListenerAsync<TransactionCommittedEvent>,
        IListenerAsync<TransactionCommittingEvent>
    {
        private readonly ConcurrentDictionary<SessionListenerType, ConcurrentDictionary<ISession, SynchronizedCollection<Func<ISession, Task>>>> _dict = 
            new ConcurrentDictionary<SessionListenerType, ConcurrentDictionary<ISession, SynchronizedCollection<Func<ISession, Task>>>>();

        public Task Handle(TransactionCommittedEvent e)
        {
            return HandleEvent(SessionListenerType.AfterCommit, e.Message);
        }

        public Task Handle(TransactionCommittingEvent e)
        {
            return HandleEvent(SessionListenerType.BeforeCommit, e.Message);
        }

        public void AddAListener(SessionListenerType type, ISession session, Func<Task> action)
        {
            AddAListener(type, session, s => action());
        }

        public void AddAListener(SessionListenerType type, ISession session, Func<ISession, Task> action)
        {
            if (!_dict.ContainsKey(type))
                _dict.GetOrAdd(type, o => new ConcurrentDictionary<ISession, SynchronizedCollection<Func<ISession, Task>>>());

            if (!_dict[type].ContainsKey(session))
                _dict[type].GetOrAdd(session, o => new SynchronizedCollection<Func<ISession, Task>>());
            _dict[type][session].Add(action);
        }

        private async Task HandleEvent(SessionListenerType type, ISession session)
        {
            if (!_dict.ContainsKey(type) || !_dict[type].ContainsKey(session))
            {
                return;
            }

            foreach (var action in _dict[type][session])
            {
                await action(session).ConfigureAwait(false);
            }
            _dict[type][session].Clear();
            SynchronizedCollection<Func<ISession, Task>> ss;
            if(!_dict[type].TryRemove(session, out ss))
                throw new Exception("TryRemove session failed");
        }
    }
}
