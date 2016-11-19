using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;
using NHibernate;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using PowerArhitecture.Common.Exceptions;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.DataAccess.NHEventListeners
{
    /// <summary>
    /// Update the audit properties for the persistent entities (transient entities are handled in NhSaveOrUpdateEventListener)
    /// </summary>
    internal class AuditEntityEventListener : IPreUpdateEventListener
    {
        private readonly IAuditUserProvider _auditUserProvider;

        public AuditEntityEventListener(IAuditUserProvider auditUserProvider)
        {
            _auditUserProvider = auditUserProvider;
        }

        public async Task<bool> OnPreUpdateAsync(PreUpdateEvent @event)
        {
            var userType = UpdateAuditProperties(@event.Entity, @event.Persister, @event.State);
            if (userType != null)
            {
                Set(@event.Entity, @event.Persister, @event.State, "LastModifiedBy", await GetCurrentUserAsync(@event.Session, userType));
            }
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var userType = UpdateAuditProperties(@event.Entity, @event.Persister, @event.State);
            if (userType != null)
            {
                Set(@event.Entity, @event.Persister, @event.State, "LastModifiedBy", GetCurrentUser(@event.Session, userType));
            }
            return false;
        }

        private object GetCurrentUser(ISession session, Type userType)
        {
            var currentUser = _auditUserProvider.GetCurrentUser(session, userType);
            if (currentUser == null)
            {
                throw new PowerArhitectureException("IAuditUserProvider failed to get the current user");
            }
            return currentUser;
        }

        private async Task<object> GetCurrentUserAsync(ISession session, Type userType)
        {
            var currentUser = await _auditUserProvider.GetCurrentUserAsync(session, userType);
            if (currentUser == null)
            {
                throw new PowerArhitectureException("IAuditUserProvider failed to get the current user");
            }
            return currentUser;
        }

        private Type UpdateAuditProperties(object obj, IEntityPersister persister, object[] state)
        {
            var entity = obj as IEntity;
            if (entity == null)
            {
                return null;
            }
            var entityType = entity.GetTypeUnproxied();
            if (!typeof(IVersionedEntity).IsAssignableFrom(entityType))
            {
                return null;
            }
                
            var currentDate = DateTime.UtcNow;
            Set(entity, persister, state, "LastModifiedDate", currentDate);

            var genType = entityType.GetGenericType(typeof(IVersionedEntityWithUser<>));
            return genType?.GetGenericArguments()[0];
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
