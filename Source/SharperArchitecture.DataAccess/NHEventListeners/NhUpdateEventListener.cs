using System;
using System.Threading;
using System.Threading.Tasks;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Attributes;
using SharperArchitecture.DataAccess.Specifications;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;

namespace SharperArchitecture.DataAccess.NHEventListeners
{
    //NOTE: This class is copied from NH source only extended class was changes
    /// <summary> An event handler for update() events</summary>
    [Serializable]
    [NhEventListener(ReplaceListener = typeof(DefaultUpdateEventListener))]
    public class NhUpdateEventListener : NhSaveOrUpdateEventListener
    {
        public NhUpdateEventListener(IAuditUserProvider auditUserProvider, IEventPublisher eventPublisher)
            : base(auditUserProvider, eventPublisher)
        {
        }

        protected override async Task<object> PerformSaveOrUpdateAsync(SaveOrUpdateEvent @event, CancellationToken cancellationToken)
        {
            // this implementation is supposed to tolerate incorrect unsaved-value
            // mappings, for the purpose of backward-compatibility
            EntityEntry entry = @event.Session.PersistenceContext.GetEntry(@event.Entity);
            if (entry != null)
            {
                if (entry.Status == Status.Deleted)
                {
                    throw new ObjectDeletedException("deleted instance passed to update()", null, @event.EntityName);
                }
                else
                {
                    return EntityIsPersistent(@event);
                }
            }
            else
            {
                await EntityIsDetachedAsync(@event, cancellationToken);
                return null;
            }
        }

        protected override object PerformSaveOrUpdate(SaveOrUpdateEvent @event)
        {
            // this implementation is supposed to tolerate incorrect unsaved-value
            // mappings, for the purpose of backward-compatibility
            EntityEntry entry = @event.Session.PersistenceContext.GetEntry(@event.Entity);
            if (entry != null)
            {
                if (entry.Status == Status.Deleted)
                {
                    throw new ObjectDeletedException("deleted instance passed to update()", null, @event.EntityName);
                }
                else
                {
                    return EntityIsPersistent(@event);
                }
            }
            else
            {
                EntityIsDetached(@event);
                return null;
            }
        }

        protected override Task<object> SaveWithGeneratedOrRequestedIdAsync(SaveOrUpdateEvent @event, CancellationToken cancellationToken)
        {
            return SaveWithGeneratedIdAsync(@event.Entity, @event.EntityName, null, @event.Session, true, cancellationToken);
        }

        protected override object SaveWithGeneratedOrRequestedId(SaveOrUpdateEvent @event)
        {
            return SaveWithGeneratedId(@event.Entity, @event.EntityName, null, @event.Session, true);
        }

        /// <summary> 
        /// If the user specified an id, assign it to the instance and use that, 
        /// otherwise use the id already assigned to the instance
        /// </summary>
        protected override object GetUpdateId(object entity, IEntityPersister persister, object requestedId)
        {
            if (requestedId == null)
            {
                return base.GetUpdateId(entity, persister, requestedId);
            }
            else
            {
                persister.SetIdentifier(entity, requestedId);
                return requestedId;
            }
        }
    }
}
