using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.Domain.Specifications;
using NHibernate.Persister.Entity;

namespace PowerArhitecture.Authentication.EventListeners
{
    public class UserInsertingUpdatingEventListener : IListener<EntityPreInsertingEvent>, IListener<EntityPreUpdatingEvent>
    {
        public void Handle(EntityPreInsertingEvent e)
        {
            var @event = e.Message;
            var user = @event.Entity as User;
            if (user == null) return;
            Set(@event.Persister, @event.State, o => o.SecurityStamp, GenerateSecurityStamp());
        }

        public void Handle(EntityPreUpdatingEvent e)
        {
            var @event = e.Message;
            var user = @event.Entity as User;
            if (user == null) return;

            Set(@event.Persister, @event.State, o => o.SecurityStamp, GenerateSecurityStamp());
        }

        private string GenerateSecurityStamp()
        {
            return Guid.NewGuid().ToString();
        }

        private static void Set(IEntityPersister persister, object[] state, Expression<Func<User, object>> memberExp, object value)
        {
            var propertyName = ExpressionHelper.GetExpressionPath(memberExp.Body);
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}
