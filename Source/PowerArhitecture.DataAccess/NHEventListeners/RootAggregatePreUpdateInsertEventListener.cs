using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain.Specifications;

namespace PowerArhitecture.DataAccess.NHEventListeners
{
    public class RootAggregatePreUpdateInsertEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        public async Task<bool> OnPreUpdate(PreUpdateEvent @event)
        {
            await LockRoot(@event.Entity, @event.Session).ConfigureAwait(false);
            return false;
        }

        public async Task<bool> OnPreInsert(PreInsertEvent @event)
        {
            await LockRoot(@event.Entity, @event.Session).ConfigureAwait(false);
            return false;
        }

        private async Task LockRoot(object entity, IEventSource session)
        {
            var currentChild = entity as IAggregateChild;
            if (currentChild == null) return;
            
            while (currentChild != null)
            {
                var root = currentChild.AggregateRoot;
                if(root == null) break;
                var rootEntry = session.PersistenceContext.GetEntry(root);
                if (rootEntry == null || !LockMode.Force.Equals(rootEntry.LockMode))
                {
                    //await session.LockAsync(root, LockMode.Force);
                    await Task.Yield();
                }
                currentChild = root as IAggregateChild;
            }
        }
    }
}
