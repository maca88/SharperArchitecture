using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Settings;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;

namespace PowerArhitecture.DataAccess.Conventions.Mssql
{
    public class MssqlHiLoIdConvention : IIdConvention, ICreateSchemaConvention
    {
        private const string NextHiValueColumnName = "NextHiValue";
        private const string TableColumnName = "Entity";
        private readonly string _hiLoIdentityTableName;
        private readonly string _maxLo;
        private readonly ConventionsSettings _settings;
        private readonly Type[] _validTypes = new [] { typeof(int), typeof(long), typeof(uint), typeof(ulong) };
        private readonly HashSet<string> _validDialects = new HashSet<string>
            {
                typeof (MsSql2000Dialect).FullName,
                typeof (MsSql2005Dialect).FullName,
                typeof (MsSql2008Dialect).FullName
            };

        public MssqlHiLoIdConvention(ConventionsSettings conventionsSettings)
        {
            _settings = conventionsSettings;
            _hiLoIdentityTableName = conventionsSettings.HiLoId.TableName;
            _maxLo = conventionsSettings.HiLoId.MaxLo.ToString(CultureInfo.InvariantCulture);
        }

        public void Apply(IIdentityInstance instance)
        {
            if(instance.Property == null || !_validTypes.Contains(instance.Property.PropertyType)) return;
            instance.GeneratedBy.HiLo(_hiLoIdentityTableName, NextHiValueColumnName, _maxLo, builder =>
                builder.AddParam("where", string.Format("{0} = '[{1}]'", TableColumnName, instance.EntityType.Name)));
        }

        public bool CanApply(Dialect dialect)
        {
            return _settings.HiLoId.Enabled && _validDialects.Contains(dialect.GetType().FullName);
        }

        public void ApplyBeforeSchemaCreate(Configuration config, IDbConnection connection)
        {
            var script = new StringBuilder();
            script.AppendFormat("DELETE FROM {0};", _hiLoIdentityTableName);
            script.AppendLine();
            script.AppendFormat("ALTER TABLE {0} ADD {1} VARCHAR(128) NOT NULL;", _hiLoIdentityTableName, TableColumnName);
            script.AppendLine();
            script.AppendFormat("CREATE NONCLUSTERED INDEX IX_{0}_{1} ON {0} (Entity DESC);", _hiLoIdentityTableName, TableColumnName);
            script.AppendLine();
            script.AppendLine("GO");
            script.AppendLine();
            foreach (var entityName in config.ClassMappings.Select(m => m.MappedClass != null ? m.MappedClass.Name : m.Table.Name).Distinct())
            {
                script.AppendFormat("INSERT INTO [{0}] ({1}, {2}) VALUES ('[{3}]', 0);", _hiLoIdentityTableName, TableColumnName, NextHiValueColumnName, entityName);
                script.AppendLine();
            }

            config.AddAuxiliaryDatabaseObject(new SimpleAuxiliaryDatabaseObject(script.ToString(), null, _validDialects));
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
