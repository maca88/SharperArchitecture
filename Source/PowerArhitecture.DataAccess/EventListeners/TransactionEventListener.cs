using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Transaction;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;

namespace PowerArhitecture.DataAccess.EventListeners
{
    public class TransactionEventListener : ISynchronization
    {
        private readonly ISession _session;
        private readonly IEventAggregator _eventAggregator;

        public TransactionEventListener(ISession session, IEventAggregator eventAggregator)
        {
            _session = session;
            _eventAggregator = eventAggregator;
        }

        public async Task BeforeCompletion()
        {
            await _eventAggregator.SendMessageAsync(new TransactionCommittingEvent(_session));
        }

        public async Task AfterCompletion(bool success)
        {
            await _eventAggregator.SendMessageAsync(new TransactionCommittedEvent(_session));
        }
    }
}
