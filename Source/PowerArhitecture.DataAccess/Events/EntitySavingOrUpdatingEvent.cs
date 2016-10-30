using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    public class EntitySavingOrUpdatingEvent : IEvent
    {
        public EntitySavingOrUpdatingEvent(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }
    }
}
