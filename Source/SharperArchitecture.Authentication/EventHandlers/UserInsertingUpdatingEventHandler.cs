using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using SharperArchitecture.Authentication.Specifications;

namespace SharperArchitecture.Authentication.EventHandlers
{
    public class UserInsertingUpdatingEventHandler :
        IPreInsertEventListener,
        IPreUpdateEventListener
    {
        private string GenerateSecurityStamp()
        {
            return Guid.NewGuid().ToString();
        }

        private static void Set(IEntityPersister persister, object[] state, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, "SecurityStamp");
            if (index == -1)
                return;
            state[index] = value;
        }

        public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
        {
            return Task.FromResult(OnPreInsert(@event));
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            var user = @event.Entity as IUser;
            if (user == null) return false;
            Set(@event.Persister, @event.State, GenerateSecurityStamp());
            return false;
        }

        public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
        {
            return Task.FromResult(OnPreUpdate(@event));
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var user = @event.Entity as IUser;
            if (user == null)
            {
                return false;
            }
            Set(@event.Persister, @event.State, GenerateSecurityStamp());
            return false;
        }
    }
}
