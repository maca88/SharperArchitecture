using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider.NH.Metadata;
using NHibernate;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Breeze.Events
{
    public class BreezeMetadataBuiltEvent : IEvent
    {
        public BreezeMetadataBuiltEvent(MetadataSchema metadata, ISessionFactory sessionFactory)
        {
            Metadata = metadata;
            SessionFactory = sessionFactory;
        }

        public MetadataSchema Metadata { get; }

        public ISessionFactory SessionFactory { get; }
    }
}
