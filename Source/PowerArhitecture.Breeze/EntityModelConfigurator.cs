using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider.NH;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PowerArhitecture.Breeze.Specification;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;

namespace PowerArhitecture.Breeze
{
    public class EntityModelConfigurator : IBreezeModelConfigurator
    {
        public void Configure()
        {
            BreezeModelConfigurator.Configure<Entity<int>>()
                .ForMember<long>(o => o.Id, o => o.Writable(true));

            BreezeModelConfigurator.Configure<Entity<long>>()
                .ForMember<long>(o => o.Id, o => o.Writable(true));

            BreezeModelConfigurator.Configure<Entity<string>>()
                .ForMember<string>(o => o.Id, o => o.Writable(true));

            BreezeModelConfigurator.Configure<IVersionedEntity>()
                .ForMember(o => o.Version, o => o.Writable(true))
                .ForMember(o => o.LastModifiedDate, o => o.Writable(true))
                .ForMember(o => o.CreatedDate, o => o.Writable(true));

            //BreezeModelConfigurator.Configure<VersionedEntity<>>()
            //    .ForMember<IUser>(o => o.LastModifiedBy, o => o.Writable(false).Readable(true))
            //    .ForMember<IUser>(o => o.CreatedBy, o => o.Writable(false).Readable(true).Include());

            //BreezeModelConfigurator.Configure<VersionedEntity<string, IUser>>()
            //    .ForMember<IUser>(o => o.LastModifiedBy, o => o.Writable(false).Readable(true))
            //    .ForMember<IUser>(o => o.CreatedBy, o => o.Writable(false).Readable(true).Include());
        }
    }
}
