using System;
using System.Collections.Generic;
using System.Security.Principal;
using SharperArchitecture.Authentication.Configurations;
using SharperArchitecture.Authentication.Entities;
using SharperArchitecture.Authentication.Specifications;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using SharperArchitecture.Common.Configuration;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Authentication.Convention
{
    public class PrincipalConvention : IReferenceConvention, IPropertyConvention, IClassConvention
    {
        private readonly Type _userType;
        private readonly ConventionsConfiguration _configuration;

        public PrincipalConvention(ConventionsConfiguration configuration)
        {
            _configuration = configuration;
            _userType = Package.UserType;
        }

        public void Apply(IManyToOneInstance instance)
        {
            if (!typeof (IPrincipal).IsAssignableFrom(instance.Property.PropertyType)) return;

            instance.OverrideInferredClass(_userType);

            if (!_userType.IsAssignableFrom(instance.EntityType)) return;
            instance.Nullable();
        }

        public void Apply(IPropertyInstance instance)
        {
            var set = new HashSet<string> { "CreatedById", "LastModifiedById" };
            if (!set.Contains(instance.Property.Name) ||
                !instance.Property.DeclaringType.IsAssignableToGenericType(typeof(VersionedEntityWithUser<,>)))
            {
                return;
            }
            instance.CustomType(typeof(long));

            if (!_configuration.RequiredLastModifiedProperty && instance.Property.Name == "LastModifiedById")
            {
                return;
            }
            // User must have nullable CreatedById and LastModifiedById otherwise we wont be able to insert any record
            if (!_userType.IsAssignableFrom(instance.EntityType))
            {
                instance.Not.Nullable();
            }
            else
            {
                instance.Nullable();
            }
        }

        public void Apply(IClassInstance instance)
        {
        }
    }
}
