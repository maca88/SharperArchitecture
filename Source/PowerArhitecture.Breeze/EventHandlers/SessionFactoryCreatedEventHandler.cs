using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider.NH;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Events;

namespace PowerArhitecture.Breeze.EventHandlers
{
    public class SessionFactoryCreatedEventHandler : IEventHandler<SessionFactoryCreatedEvent>
    {
        public void Handle(SessionFactoryCreatedEvent @event)
        {
            NHibernateContractResolver.Instance.RegisterSessionFactory(@event.SessionFactory);
        }
    }
}
