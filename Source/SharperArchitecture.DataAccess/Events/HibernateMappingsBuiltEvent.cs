using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.MappingModel;
using NHibernate.Cfg;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
{
    public class HibernateMappingsBuiltEvent : IEvent
    {
        public HibernateMappingsBuiltEvent(Configuration configuration, IEnumerable<HibernateMapping> mappings)
        {
            Configuration = configuration;
            Mappings = mappings;
        }

        public IEnumerable<HibernateMapping> Mappings { get; }

        public Configuration Configuration { get; set; }
    }
}
