using System;
using System.Collections.Generic;
using Breeze.ContextProvider.NH;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;

namespace PowerArhitecture.Breeze
{
    public class EntityModelConfiguratorTask : IStartupTask
    {
        private readonly IBreezeConfigurator _breezeConfigurator;

        public EntityModelConfiguratorTask(IBreezeConfigurator breezeConfigurator)
        {
            _breezeConfigurator = breezeConfigurator;
        }

        public void Run()
        {
            _breezeConfigurator.Configure<Entity<int>>()
                .ForMember<long>(o => o.Id, o => o.Writable(true));

            _breezeConfigurator.Configure<Entity<long>>()
                .ForMember(o => o.Id, o => o.Writable(true));

            _breezeConfigurator.Configure<Entity<string>>()
                .ForMember(o => o.Id, o => o.Writable(true));

            _breezeConfigurator.Configure<IVersionedEntity>()
                .ForMember(o => o.Version, o => o.Writable(true))
                .ForMember(o => o.LastModifiedDate, o => o.Writable(true))
                .ForMember(o => o.CreatedDate, o => o.Writable(true));
        }
    }
}
