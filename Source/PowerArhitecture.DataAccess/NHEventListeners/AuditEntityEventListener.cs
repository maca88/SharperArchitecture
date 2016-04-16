using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;
using NHibernate;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.DataAccess.NHEventListeners
{
    /// <summary>
    /// Update the audit properties for the persistent entities (transient entities are handled in NhSaveOrUpdateEventListener)
    /// </summary>
    public class AuditEntityEventListener : IPreUpdateEventListener
    {
        private readonly IAuditUserProvider _auditUserProvider;

        public AuditEntityEventListener(IAuditUserProvider auditUserProvider)
        {
            _auditUserProvider = auditUserProvider;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            EditEntity(@event.Entity, @event.Persister, @event.Session, @event.State);
            return false;
        }


        private void EditEntity(object obj, IEntityPersister persister, ISession session, object[] state)
        {
            var entity = obj as IEntity;
            if(entity == null) return;
            var entityType = entity.GetTypeUnproxied();
            if (!typeof(IVersionedEntity).IsAssignableFrom(entityType)) 
                return;

            var currentDate = DateTime.UtcNow;
            Set(entity, persister, state, "LastModifiedDate", currentDate);

            var genType = entityType.GetGenericType(typeof (IVersionedEntityWithUser<>));
            if (genType == null) 
                return;

            var userType = genType.GetGenericArguments()[0]; //The first 
            var currentUser = _auditUserProvider.GetCurrentUser(session, userType);
            Set(entity, persister, state, "LastModifiedBy", currentUser);
        }

        /// <summary>
        /// Update the entity state values and the entity property value
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="persister"></param>
        /// <param name="state"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        private static void Set(object entity, IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
            entity.SetMemberValue(propertyName, value);
        }

    }
}
