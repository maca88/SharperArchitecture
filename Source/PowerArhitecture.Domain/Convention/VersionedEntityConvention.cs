using System;
using System.Security.Principal;
using PowerArhitecture.Domain.Specifications;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.Domain.Convention
{
    public class VersionedEntityConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            if (!instance.EntityType.IsAssignableToGenericType(typeof(IVersionedEntity<>)) ||
                !typeof (IPrincipal).IsAssignableFrom(instance.Property.PropertyType)) 
                return;

            instance.Cascade.None(); //CreatedBy LastModifiedBy
        }
    }
}
