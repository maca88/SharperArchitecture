using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.MappingModel;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    public class HibernateMappingsBuiltEvent : IEvent
    {
        public HibernateMappingsBuiltEvent(IEnumerable<HibernateMapping> mappings)
        {
            Mappings = mappings;
        }

        public IEnumerable<HibernateMapping> Mappings { get; }
    }
}
