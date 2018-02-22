using System.Collections.Generic;
using FluentNHibernate.Automapping;
using FluentNHibernate.MappingModel;
using NHibernate.Cfg;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Events;

namespace SharperArchitecture.DataAccess.Decorators
{
    public class CustomAutoPersistenceModel: AutoPersistenceModel
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Configuration _configuration;

        public CustomAutoPersistenceModel(Configuration configuration, IEventPublisher eventPublisher, IAutomappingConfiguration cfg) : base(cfg)
        {
            _eventPublisher = eventPublisher;
            _configuration = configuration;
        }

        public override IEnumerable<HibernateMapping> BuildMappings()
        {
            var message = base.BuildMappings();
            _eventPublisher.Publish(new HibernateMappingsBuiltEvent(_configuration, message));
            return message;
        }
    }
}
