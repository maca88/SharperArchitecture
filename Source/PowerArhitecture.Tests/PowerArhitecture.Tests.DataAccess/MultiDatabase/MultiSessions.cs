using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.DataAccess.Attributes;

namespace PowerArhitecture.Tests.DataAccess.MultiDatabase
{
    public class MultiSessions
    {
        public MultiSessions(ISession session, [Database("bar")]ISession barSession)
        {
            DefaultSession = session;
            BarSession = barSession;
        }

        public ISession DefaultSession { get; }

        public ISession BarSession { get; }
    }
}
