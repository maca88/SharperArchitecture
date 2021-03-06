﻿using System;
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
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;
using SharperArchitecture.Domain.Specifications;

namespace SharperArchitecture.DataAccess.Conventions
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
                !new List<string> { "CreatedBy", "LastModifiedBy" }.Contains(instance.Property.Name) ||
                !instance.EntityType.IsAssignableToGenericType(typeof(IVersionedEntityWithUser<>))
            )
            {
                return;
            }
            instance.Cascade.None(); //CreatedBy LastModifiedBy
            if (instance.Property.Name == "CreatedBy")
            {
                instance.Not.Nullable();
            }
            else if (_configuration.RequiredLastModifiedProperty && instance.Property.Name == "LastModifiedBy")
            {
                instance.Not.Nullable();
            }
        }

        public void Apply(IPropertyInstance instance)
        {
            if (!typeof(IVersionedEntity).IsAssignableFrom(instance.EntityType) ||
                !new List<string> {"CreatedBy", "LastModifiedBy", "LastModifiedDate" }.Contains(instance.Property.Name))
            {
                return;
            }
            if (instance.Property.Name == "CreatedBy" &&
                instance.EntityType.IsAssignableToGenericType(typeof(IVersionedEntityWithUser<>)))
            {
                instance.Not.Nullable();
            }

            if (!_configuration.RequiredLastModifiedProperty)
            {
                return;
            }
            if (
                instance.Property.Name == "LastModifiedDate" ||
                (
                    instance.Property.Name == "LastModifiedBy" && 
                    instance.EntityType.IsAssignableToGenericType(typeof(IVersionedEntityWithUser<>))
                )
            )
            {
                instance.Not.Nullable();
            }
        }
    }
}
