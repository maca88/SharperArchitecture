using System.Collections.Generic;
using FluentNHibernate.Automapping;
using FluentNHibernate.MappingModel;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Events;

namespace PowerArhitecture.DataAccess.Decorators
{
    public class CustomAutoPersistenceModel: AutoPersistenceModel
    {
        private readonly IEventPublisher _eventPublisher;

        public CustomAutoPersistenceModel(IEventPublisher eventPublisher, IAutomappingConfiguration cfg) : base(cfg)
        {
            _eventPublisher = eventPublisher;
        }

        public override IEnumerable<HibernateMapping> BuildMappings()
        {
            var message = base.BuildMappings();
            _eventPublisher.Publish(new HibernateMappingsBuiltEvent(message));
            return message;
        }
    }
}
