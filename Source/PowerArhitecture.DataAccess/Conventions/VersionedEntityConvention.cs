using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Testing.Values;
using NHibernate.Cfg;
using NHibernate.Dialect;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Specifications;

namespace PowerArhitecture.DataAccess.Conventions
{
    public class VersionedEntityConvention : IReferenceConvention, IPropertyConvention
    {
        private readonly ConventionsConfiguration _configuration;

        public VersionedEntityConvention(ConventionsConfiguration conventionsConfiguration)
        {
            _configuration = conventionsConfiguration;
        }

        public void Apply(IManyToOneInstance instance)
        {
            if (
                !instance.EntityType.IsAssignableToGenericType(typeof(IVersionedEntityWithUser<>)) ||
                !new List<string>{ "CreatedBy", "LastModifiedBy" }.Contains(instance.Property.Name)
            )
            {
                return;
            }
            instance.Cascade.None(); //CreatedBy LastModifiedBy

            if(_configuration.RequiredLastModifiedProperty && instance.Property.Name == "LastModifiedBy")
            {
                instance.Not.Nullable();
            }
        }

        public void Apply(IPropertyInstance instance)
        {
            if(_configuration.RequiredLastModifiedProperty && 
                typeof(IVersionedEntity).IsAssignableFrom(instance.EntityType) && 
                instance.Property.Name == "LastModifiedDate")
            {
                instance.Not.Nullable();
            }
        }
    }
}
