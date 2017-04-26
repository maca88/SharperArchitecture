using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
{
    public class SessionCreatedEvent : IEvent
    {
        public SessionCreatedEvent(ISession session, string dbConfigName)
        {
            Session = session;
            DatabaseConfigurationName = dbConfigName;
        }

        public string DatabaseConfigurationName { get; }

        public ISession Session { get; }
    }
}
