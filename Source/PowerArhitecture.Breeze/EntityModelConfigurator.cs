using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider.NH.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PowerArhitecture.Breeze.Specification;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Breeze
{
    public class EntityModelConfigurator : IBreezeModelConfigurator
    {
        public void Configure()
        {
            BreezeModelConfigurator.Configure<Entity<long>>()
                .ForMember<long>(o => o.Id, o => o.Writable(true));

            BreezeModelConfigurator.Configure<Entity<string>>()
                .ForMember<string>(o => o.Id, o => o.Writable(true));

            BreezeModelConfigurator.Configure<VersionedEntity>()
                .ForMember<object>(o => o.CreatedById, o => o.Writable(true))
                .ForMember<object>(o => o.LastModifiedById, o => o.Writable(true))
                .ForMember<DateTime>(o => o.LastModifiedDate, o => o.Writable(true))
                .ForMember<IUser>(o => o.LastModifiedBy, o => o.Writable(false))
                .ForMember<DateTime>(o => o.CreatedDate, o => o.Writable(true))
                .ForMember<IUser>(o => o.CreatedBy, o => o.Writable(false));

            BreezeModelConfigurator.Configure<VersionedEntity<string, IUser>>()
                .ForMember<object>(o => o.CreatedById, o => o.Writable(true))
                .ForMember<object>(o => o.LastModifiedById, o => o.Writable(true))
                .ForMember<DateTime>(o => o.LastModifiedDate, o => o.Writable(true))
                .ForMember<IUser>(o => o.LastModifiedBy, o => o.Writable(false))
                .ForMember<DateTime>(o => o.CreatedDate, o => o.Writable(true))
                .ForMember<IUser>(o => o.CreatedBy, o => o.Writable(false));
        }
    }
}
