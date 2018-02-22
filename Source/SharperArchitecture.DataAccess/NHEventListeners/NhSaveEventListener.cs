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

namespace SharperArchitecture.DataAccess.NHEventListeners
{
    //NOTE: This class is copied from NH source only extended class was changes
    /// <summary> An event handler for save() events</summary>
    [Serializable]
    [NhEventListener(ReplaceListener = typeof(DefaultSaveEventListener))]
    public class NhSaveEventListener : NhSaveOrUpdateEventListener
    {
        public NhSaveEventListener(IAuditUserProvider auditUserProvider, IEventPublisher eventPublisher)
            : base(auditUserProvider, eventPublisher)
        {
        }

        protected override object PerformSaveOrUpdate(SaveOrUpdateEvent @event)
        {
            // this implementation is supposed to tolerate incorrect unsaved-value
            // mappings, for the purpose of backward-compatibility
            var entry = @event.Session.PersistenceContext.GetEntry(@event.Entity);
            if (entry != null && entry.Status != Status.Deleted)
            {
                return EntityIsPersistent(@event);
            }
            else
            {
                return EntityIsTransient(@event);
            }
        }

        protected override async Task<object> PerformSaveOrUpdateAsync(SaveOrUpdateEvent @event, CancellationToken cancellationToken)
        {
            // this implementation is supposed to tolerate incorrect unsaved-value
            // mappings, for the purpose of backward-compatibility
            var entry = @event.Session.PersistenceContext.GetEntry(@event.Entity);
            if (entry != null && entry.Status != Status.Deleted)
            {
                return EntityIsPersistent(@event);
            }
            else
            {
                return await EntityIsTransientAsync(@event, cancellationToken);
            }
        }

        protected override bool ReassociateIfUninitializedProxy(object obj, ISessionImplementor source)
        {
            if (!NHibernateUtil.IsInitialized(obj))
            {
                throw new PersistentObjectException("Uninitialized proxy passed to save(). Object: " + obj.ToString());
            }
            else
            {
                return false;
            }
        }
    }
}
