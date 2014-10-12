using System;
using System.Linq.Expressions;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain.Specifications;
using NHibernate;
using NHibernate.Event;
using NHibernate.Persister.Entity;

namespace PowerArhitecture.DataAccess.NHEventListeners
{
    public class NhPreInsertUpdateEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        private readonly IUserCache _userCache;

        public NhPreInsertUpdateEventListener(IUserCache userCache)
        {
            _userCache = userCache;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            EditEntity(@event.Entity, @event.Persister, @event.Session, @event.State, false);
            return false;
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            EditEntity(@event.Entity, @event.Persister, @event.Session, @event.State, true);
            return false;
        }

        private void EditEntity(object entity, IEntityPersister persister, ISession session, object[] state, bool preInsert)
        {
            var verEntity = entity as IVersionedEntity<IUser>;
            if (verEntity == null)
                return;
            var user = _userCache.GetCurrentUser();
            var currentUser = user == null ? null : session.Load<IUser>(user.Id);
            var currentDate = DateTime.UtcNow;

            Set(persister, state, o => o.LastModifiedDate, currentDate);
            Set(persister, state, o => o.LastModifiedBy, currentUser);

            if (!preInsert) return;
            Set(persister, state, o => o.CreatedDate, currentDate);
            Set(persister, state, o => o.CreatedBy, currentUser);
        }

        private static void Set(IEntityPersister persister, object[] state, Expression<Func<IVersionedEntity<IUser>, object>> memberExp, object value)
        {
            var propertyName = ExpressionHelper.GetExpressionPath(memberExp.Body);
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }

    }
}
