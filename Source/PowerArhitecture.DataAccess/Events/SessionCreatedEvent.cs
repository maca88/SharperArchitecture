using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
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
