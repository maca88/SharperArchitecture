using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Testing.Values;
using NHibernate;
using NHibernate.Linq;
using PowerArhitecture.Notifications.Specifications;
using PowerArhitecture.Tests.Notifications.Entities;

namespace PowerArhitecture.Tests.Notifications.SearchQueries
{
    public class TestEntityRecipientSeachQuery :  IRecipientSearchQuery
    {
        private ISession _session;

        public TestEntityRecipientSeachQuery(ISession session)
        {
            _session = session;
        }

        public IEnumerable<object> GetRecipients(string pattern)
        {
            return new List<User>
            {
                _session.Query<User>().FirstOrDefault(o => o.UserName == "User1"),
                _session.Query<User>().FirstOrDefault(o => o.UserName == "User2")
            };
        }
    }
}
