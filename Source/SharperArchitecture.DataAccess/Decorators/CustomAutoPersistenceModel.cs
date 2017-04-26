using System.Collections.Generic;
using FluentNHibernate.Automapping;
using FluentNHibernate.MappingModel;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Events;

namespace SharperArchitecture.DataAccess.Decorators
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
