using System;
using System.Collections.Generic;
using System.Security.Principal;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.Generated;
using PowerArhitecture.Authentication.Specifications;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Convention
{
    public class PrincipalConvention : IReferenceConvention, IPropertyConvention
    {
        private readonly Type _userType = typeof(User);

        public PrincipalConvention()
        {
            var userClass = AppConfiguration.GetSetting<string>(AuthenticationSettingKeys.UserClass);
            if (!string.IsNullOrEmpty(userClass))
                _userType = Type.GetType(userClass, true);
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
                !instance.Property.DeclaringType.IsAssignableToGenericType(typeof (VersionedEntity<,>))) return;
            instance.CustomType(typeof(long));
            if (!_userType.IsAssignableFrom(instance.EntityType))
            {
                instance.Not.Nullable();
            }
            else
            {
                instance.Nullable();
            }
        }
    }
}
