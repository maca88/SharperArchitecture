using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Transaction;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Events;

namespace PowerArhitecture.DataAccess.EventListeners
{
    internal class TransactionEventListener : ISynchronization
    {
        private readonly ISession _session;
        private readonly IEventPublisher _eventPublisher;

        public TransactionEventListener(ISession session, IEventPublisher eventPublisher)
        {
            _session = session;
            _eventPublisher = eventPublisher;
        }

        public void BeforeCompletion()
        {
            _eventPublisher.Publish(new TransactionCommittingEvent(_session));
        }

        public void AfterCompletion(bool success)
        {
            _eventPublisher.Publish(new TransactionCommittedEvent(_session, success));
        }
    }
}
