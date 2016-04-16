using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.MappingModel;
using NHibernate.Cfg;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;

namespace PowerArhitecture.DataAccess.Wrappers
{
    public class CustomAutoPersistenceModel: AutoPersistenceModel
    {
        private readonly IEventAggregator _eventAggregator;

        public CustomAutoPersistenceModel(IEventAggregator eventAggregator, IAutomappingConfiguration cfg) : base(cfg)
        {
            _eventAggregator = eventAggregator;
        }

        public override IEnumerable<HibernateMapping> BuildMappings()
        {
            var message = base.BuildMappings();
            _eventAggregator.SendMessage(new HibernateMappingsBuiltEvent(message));
            return message;
        }
    }
}
