using System;
using System.Data;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Cfg;
using NHibernate.Dialect;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Specifications;

namespace SharperArchitecture.DataAccess.Conventions
{
    public class PropertyConvention : ISchemaConvention, IPropertyConvention
    {
        private readonly ConventionsConfiguration _configuration;

        public PropertyConvention(ConventionsConfiguration conventionsConfiguration)
        {
            _configuration = conventionsConfiguration;
        }

        public void Apply(IPropertyInstance instance)
        {
            if (!new[] {typeof (DateTime), typeof (DateTime?)}.Contains(instance.Property.PropertyType)) return;

            switch (_configuration.DateTimeZone.ToLowerInvariant())
            {
                case "local":
                    instance.CustomType("LocalDateTime");
                    break;
                case "utc":
                    instance.CustomType("UtcDateTime");
                    break;
            }
        }

        public bool CanApply(Dialect dialect)
        {
            return !string.IsNullOrEmpty(_configuration.DateTimeZone);
        }

        public void Setup(Configuration configuration)
        {
        }

        public void ApplyBeforeExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }

        public void ApplyAfterExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }
    }
}
