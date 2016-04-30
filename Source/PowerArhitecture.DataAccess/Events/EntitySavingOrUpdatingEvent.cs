using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.DataAccess.Events
{
    public class EntitySavingOrUpdatingEvent : BaseEvent<ISession>
    {
        public EntitySavingOrUpdatingEvent(ISession message) : base(message)
        {
        }
    }
}
