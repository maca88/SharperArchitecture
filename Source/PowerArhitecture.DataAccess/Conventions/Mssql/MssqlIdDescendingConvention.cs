using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataAccess.Settings;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace PowerArhitecture.DataAccess.Conventions.Mssql
{
    public class MssqlIdDescendingConvention : ISchemaConvention
    {
        private readonly ConventionsSettings _settings;
        private readonly HashSet<string> _validDialects = new HashSet<string>
            {
                typeof (MsSql2012Dialect).FullName,
                typeof (MsSql2008Dialect).FullName,
                typeof (MsSql2005Dialect).FullName
            };

        public MssqlIdDescendingConvention(ConventionsSettings settings)
        {
            _settings = settings;
        }

        public bool CanApply(Dialect dialect)
        {
            return _settings.IdDescending && _validDialects.Contains(dialect.GetType().FullName);
        }

        public void ApplyBeforeExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
            var match = Regex.Match(dbCommand.CommandText, @"create[\s]+table[\s]+([\[\]\w]+)[\s\w\(,\)[\[\]]+((?:primary\s+key\s+)\(([^\)]+)\))");
            if (!match.Success) return;
            var tableName = match.Groups[1].Value.TrimEnd(']').TrimStart('[');
            var pkConstraintOld = match.Groups[2].Value;
            var columns = match.Groups[3].Value.Split(',').Select(o => string.Format("{0} DESC", o.Trim())).ToList();
            var pkConstraintNew = string.Format("CONSTRAINT {0} PRIMARY KEY ({1})", GetPrimaryKeyName(tableName), string.Join(", ", columns));
            dbCommand.CommandText = dbCommand.CommandText.Replace(pkConstraintOld, pkConstraintNew);
        }

        private static string GetPrimaryKeyName(string tableName)
        {
            return string.Format("PK_{0}", tableName);
        }

        public void ApplyAfterExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }
    }
}
