using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.DataAccess.Attributes;

namespace PowerArhitecture.Tests.DataAccess.MultiDatabase
{
    public class MultiSessionFactories
    {
        public MultiSessionFactories(ISessionFactory sessionFactory, [Database("bar")]ISessionFactory barSessionFactory)
        {
            DefaultSessionFactory = sessionFactory;
            BarSessionFactory = barSessionFactory;
        }

        public ISessionFactory DefaultSessionFactory { get; }

        public ISessionFactory BarSessionFactory { get; }
    }
}
