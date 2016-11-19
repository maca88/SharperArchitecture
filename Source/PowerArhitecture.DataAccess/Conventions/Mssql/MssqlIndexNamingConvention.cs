using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain.Attributes.Mssql;

namespace PowerArhitecture.DataAccess.Conventions.Mssql
{
    public class MssqlIndexNamingConvention : ISchemaConvention
    {
        private readonly ConventionsConfiguration _configuration;
        private static readonly IInternalLogger Logger = LoggerProvider.LoggerFor(typeof(SchemaExport));
        private readonly HashSet<string> _validDialects = new HashSet<string>
            {
                typeof (MsSql2012Dialect).FullName,
                typeof (MsSql2008Dialect).FullName
            };

        public MssqlIndexNamingConvention(ConventionsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool CanApply(Dialect dialect)
        {
            return _configuration.UniqueWithMultipleNulls && _validDialects.Contains(dialect.GetType().FullName);
        }

        public void Setup(Configuration configuration)
        {
        }

        public void ApplyBeforeExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
            var indexMatch = Regex.Match(dbCommand.CommandText, @"create\s+index\s+([\w\d]+)\s+on\s+([\w\d\[\]]+)\s+\(([\w\d\s\[\],]+)\)");
            if(!indexMatch.Success) return;

            var oldIndexName = indexMatch.Groups[1].Value.TrimStart('[').TrimEnd(']').Split('_').Last();
            var tableName = indexMatch.Groups[2].Value.TrimStart('[').TrimEnd(']');
            var columns = indexMatch.Groups[3].Value.Split(',').Select(o => o.Trim()).ToList();
            var key = GetUniqueKeyName(tableName, columns);
            dbCommand.CommandText = dbCommand.CommandText.Replace(indexMatch.Groups[1].Value, key);

            var classMap = config.ClassMappings.FirstOrDefault(o => o.RootTable?.Name == tableName && o.MappedClass != null);
            var indexAttribute = classMap?.MappedClass?.GetProperties()
                    .Select(o => new { Property = o, Index = o.GetCustomAttribute<MssqlIndexAttribute>() })
                    .FirstOrDefault(o => o.Index?.Include != null && (o.Index.KeyName == oldIndexName || o.Property.Name == oldIndexName));
            if (indexAttribute != null)
            {
                dbCommand.CommandText += $" include ({string.Join(",", indexAttribute.Index.Include)})";
            }
        }

        public void ApplyAfterExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }

        private static string GetUniqueKeyName(string tableName, IEnumerable<string> columnNames)
        {
            return string.Format("IX_{0}_{1}", tableName, string.Join("_", columnNames.Select(o => o.TrimEnd(']').TrimStart('['))));
        }
    }
}
