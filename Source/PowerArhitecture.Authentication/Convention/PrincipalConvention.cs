using System.Security.Principal;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.DataAccess.Settings;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace PowerArhitecture.Authentication.Convention
{
    public class PrincipalConvention : IReferenceConvention
    {
        private readonly ConventionsSettings _settings;

        public PrincipalConvention(ConventionsSettings conventionsSettings)
        {
            _settings = conventionsSettings;

        }

        public void Apply(IManyToOneInstance instance)
        {
            if (!typeof (IPrincipal).IsAssignableFrom(instance.Property.PropertyType)) return;

            instance.OverrideInferredClass(typeof(User));

            if(!typeof (User).IsAssignableFrom(instance.EntityType)) return;
            instance.Nullable();
        }


        /*
        public bool CanApply(Dialect dialect)
        {
            return _settings.UseBuiltInPrincipal;
        }

        public void ApplyBeforeExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }

        public void ApplyAfterExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }

        public void ApplyBeforeSchemaCreate(Configuration config, IDbConnection connection)
        {
            var userMapping = config.ClassMappings.First(o => o.MappedClass == typeof (User));
            userMapping.GetProperty("CreatedBy").ColumnIterator.OfType<Column>().First().IsNullable = true;
            userMapping.GetProperty("LastModifiedBy").ColumnIterator.OfType<Column>().First().IsNullable = true;
        }

        public void ApplyAfterSchemaCreate(Configuration config, IDbConnection connection)
        {
        }*/
    }
}
