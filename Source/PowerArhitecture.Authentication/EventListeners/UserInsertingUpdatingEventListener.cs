using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Event;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.Domain.Specifications;
using NHibernate.Persister.Entity;

namespace PowerArhitecture.Authentication.EventListeners
{
    public class UserInsertingUpdatingEventListener :
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

        public Task<bool> OnPreInsert(PreInsertEvent @event)
        {
            var user = @event.Entity as IUser;
            if (user == null) return Task.FromResult(false);
            Set(@event.Persister, @event.State, GenerateSecurityStamp());
            return Task.FromResult(false);
        }

        public Task<bool> OnPreUpdate(PreUpdateEvent @event)
        {
            var user = @event.Entity as IUser;
            if (user == null) return Task.FromResult(false);
            Set(@event.Persister, @event.State, GenerateSecurityStamp());
            return Task.FromResult(false);
        }
    }
}
