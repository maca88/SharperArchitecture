using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Testing.Values;
using PowerArhitecture.Notifications.Specifications;

namespace PowerArhitecture.Tests.Notifications.SearchQueries
{
    public class TestStringRecipientSeachQuery :  IRecipientSearchQuery
    {
        public IEnumerable<object> GetRecipients(string pattern)
        {
            return new List<string> { pattern + "1", pattern + "2" };
        }
    }
}
