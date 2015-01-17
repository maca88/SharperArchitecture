using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.MappingModel;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.DataAccess.Events
{
    public class HibernateMappingsBuiltEvent : BaseEvent<IEnumerable<HibernateMapping>>
    {
        public HibernateMappingsBuiltEvent(IEnumerable<HibernateMapping> message)
            : base(message)
        {
        }
    }
}
