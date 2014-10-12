using System.Data;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;
using NHibernate;
using NHibernate.Transaction;

namespace PowerArhitecture.DataAccess.Wrappers
{
    public class TransactionWrapper : ITransaction
    {
        private readonly ITransaction _transaction;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISession _session;

        public TransactionWrapper(ITransaction transaction, IEventAggregator eventAggregator, ISession session)
        {
            _session = session;
            _transaction = transaction;
            _eventAggregator = eventAggregator;
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public void Begin()
        {
            _transaction.Begin();
            _eventAggregator.SendMessage(new TransactionBeganEvent(_session));
        }

        public void Begin(IsolationLevel isolationLevel)
        {
            _transaction.Begin(isolationLevel);
            _eventAggregator.SendMessage(new TransactionBeganEvent(_session));
        }

        public void Commit()
        {
            _eventAggregator.SendMessage(new TransactionCommittingEvent(_session));
            _transaction.Commit();
            _eventAggregator.SendMessage(new TransactionCommittedEvent(_session));
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Enlist(IDbCommand command)
        {
            _transaction.Enlist(command);
        }

        public void RegisterSynchronization(ISynchronization synchronization)
        {
            _transaction.RegisterSynchronization(synchronization);
        }

        public bool IsActive { get { return _transaction.IsActive; } }
        public bool WasRolledBack { get { return _transaction.WasRolledBack; } }
        public bool WasCommitted { get { return _transaction.WasCommitted; } }
    }
}
