using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.MappingModel;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
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
