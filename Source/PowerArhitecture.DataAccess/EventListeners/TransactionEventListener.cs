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

        public void BeforeCompletion()
        {
            _eventAggregator.SendMessage(new TransactionCommittingEvent(_session));
        }

        public void AfterCompletion(bool success)
        {
            _eventAggregator.SendMessage(new TransactionCommittedEvent(_session));
        }
    }
}
