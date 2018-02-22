using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Util;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Internal;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain.Attributes;

namespace SharperArchitecture.DataAccess.Conventions
{
    public abstract class BaseHiLoIdConvention : IIdConvention, ICreateSchemaConvention
    {
        private readonly string _maxLo;
        private readonly ConventionsConfiguration _configuration;
        private readonly Type[] _validTypes = { typeof(int), typeof(long), typeof(uint), typeof(ulong) };

        protected BaseHiLoIdConvention(ConventionsConfiguration conventionsConfiguration)
        {
            _configuration = conventionsConfiguration;
            HiLoIdentityTableName = conventionsConfiguration.HiLoId.TableName;
            _maxLo = conventionsConfiguration.HiLoId.MaxLo.ToString(CultureInfo.InvariantCulture);
        }

        protected INamingStrategy NamingStrategy { get; private set; }

        protected Dialect Dialect { get; private set; }

        protected string HiLoIdentityTableName { get; private set; }

        protected string TableColumnName { get; private set; } = "Entity";

        protected string NextHiValueColumnName { get; private set; } = "NextHiValue";

        protected abstract Type GetBaseDialect();

        public abstract string CreateScript(IEnumerable<string> entityNames);

        public void Apply(IIdentityInstance instance)
        {
            if (instance.Property == null || !Enumerable.Contains(_validTypes, instance.Property.PropertyType))
            {
                return;
            }
            var maxLo = _maxLo;
            var attr = instance.EntityType.GetCustomAttribute<HiLoAttribute>();
            if (attr != null)
            {
                if (attr.Disabled)
                {
                    return;
                }
                maxLo = attr.MaxLo.ToString(CultureInfo.InvariantCulture);
            }
            instance.GeneratedBy.HiLo(HiLoIdentityTableName, NextHiValueColumnName, maxLo, 
                builder => builder.AddParam("where", $"{TableColumnName} = '[{instance.EntityType.Name}]'"));
        }

        public bool CanApply(Dialect dialect)
        {
            return _configuration.HiLoId.Enabled && GetBaseDialect().IsInstanceOfType(dialect);
        }

        public void Setup(Configuration config)
        {
            Dialect = Dialect.GetDialect(config.Properties);
            // Setup column/table names before the IIdConvention.Apply is called
            NamingStrategy = config.NamingStrategy;
            HiLoIdentityTableName = ConvertQuotes(NamingStrategy.TableName(HiLoIdentityTableName));
            TableColumnName = ConvertQuotes(NamingStrategy.ColumnName(TableColumnName));
            NextHiValueColumnName = ConvertQuotes(NamingStrategy.ColumnName(NextHiValueColumnName));
        }

        protected string ConvertQuotes(string name)
        {
            if (name.StartsWith("`") && name.EndsWith("`"))
            {
                return $"{Dialect.OpenQuote}{name.Trim('`')}{Dialect.CloseQuote}";
            }
            return name;
        }

        protected string WithoutQuotes(string name)
        {
            return name.Trim('`').TrimStart(Dialect.OpenQuote).TrimEnd(Dialect.CloseQuote);
        }

        public void ApplyBeforeSchemaCreate(Configuration config, IDbConnection connection)
        {
            var entityNames = config.ClassMappings.Select(m => m.MappedClass?.Name ?? m.Table.Name).Distinct().ToList();
            if (!entityNames.Any())
            {
                return;
            }
            config.AddAuxiliaryDatabaseObject(new CustomAuxiliaryDatabaseObject(CreateScript(entityNames), null, GetBaseDialect()));
        }

        public void ApplyBeforeExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }

        public void ApplyAfterExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }

        public void ApplyAfterSchemaCreate(Configuration config, IDbConnection connection)
        {
        }
    }
}
