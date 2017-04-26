using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Specifications;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;

namespace SharperArchitecture.DataAccess.Conventions.Mssql
{
    public class MssqlHiLoIdConvention : IIdConvention, ICreateSchemaConvention
    {
        private const string NextHiValueColumnName = "NextHiValue";
        private const string TableColumnName = "Entity";
        private readonly string _hiLoIdentityTableName;
        private readonly string _maxLo;
        private readonly ConventionsConfiguration _configuration;
        private readonly Type[] _validTypes = new [] { typeof(int), typeof(long), typeof(uint), typeof(ulong) };
        private readonly HashSet<string> _validDialects = new HashSet<string>
            {
                typeof (MsSql2000Dialect).FullName,
                typeof (MsSql2005Dialect).FullName,
                typeof (MsSql2008Dialect).FullName
            };

        public MssqlHiLoIdConvention(ConventionsConfiguration conventionsConfiguration)
        {
            _configuration = conventionsConfiguration;
            _hiLoIdentityTableName = conventionsConfiguration.HiLoId.TableName;
            _maxLo = conventionsConfiguration.HiLoId.MaxLo.ToString(CultureInfo.InvariantCulture);
        }

        public void Apply(IIdentityInstance instance)
        {
            if(instance.Property == null || !_validTypes.Contains(instance.Property.PropertyType)) return;
            instance.GeneratedBy.HiLo(_hiLoIdentityTableName, NextHiValueColumnName, _maxLo, builder =>
                builder.AddParam("where", string.Format("{0} = '[{1}]'", TableColumnName, instance.EntityType.Name)));
        }

        public bool CanApply(Dialect dialect)
        {
            return _configuration.HiLoId.Enabled && _validDialects.Contains(dialect.GetType().FullName);
        }

        public void Setup(Configuration config)
        {
            var entityNames = config.ClassMappings.Select(m => m.MappedClass?.Name ?? m.Table.Name).Distinct().ToList();
            if (!entityNames.Any())
            {
                return;
            }

            var createScript = new StringBuilder();
            createScript.AppendFormat("DELETE FROM {0};", _hiLoIdentityTableName);
            createScript.AppendLine();
            createScript.AppendFormat("ALTER TABLE {0} ADD {1} VARCHAR(128) NOT NULL;", _hiLoIdentityTableName, TableColumnName);
            createScript.AppendLine();
            createScript.AppendFormat("CREATE NONCLUSTERED INDEX IX_{0}_{1} ON {0} ({1} DESC);", _hiLoIdentityTableName, TableColumnName);
            createScript.AppendLine();
            createScript.AppendLine("GO");
            createScript.AppendLine();
            foreach (var entityName in entityNames)
            {
                createScript.AppendFormat("INSERT INTO [{0}] ({1}, {2}) VALUES ('[{3}]', 0);", _hiLoIdentityTableName, TableColumnName, 
                    NextHiValueColumnName, entityName);
                createScript.AppendLine();
            }
            config.AddAuxiliaryDatabaseObject(new SimpleAuxiliaryDatabaseObject(createScript.ToString(), null, _validDialects));
        }

        public void ApplyBeforeSchemaCreate(Configuration config, IDbConnection connection)
        {
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
