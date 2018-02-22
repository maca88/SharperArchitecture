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
using NHibernate.Mapping;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain.Attributes;

namespace SharperArchitecture.DataAccess.Conventions.Postgre
{
    public class PostgreHiLoIdConvention : BaseHiLoIdConvention
    {
        public PostgreHiLoIdConvention(ConventionsConfiguration conventionsConfiguration) : base(conventionsConfiguration)
        {
        }

        protected override Type GetBaseDialect()
        {
            return typeof(PostgreSQLDialect);
        }

        public override string CreateScript(IEnumerable<string> entityNames)
        {
            var script = new StringBuilder();
            script.AppendFormat("DELETE FROM {0};", HiLoIdentityTableName);
            script.AppendLine();
            script.AppendFormat("ALTER TABLE {0} ADD COLUMN {1} VARCHAR(128) NOT NULL;", HiLoIdentityTableName, TableColumnName);
            script.AppendLine();
            script.AppendFormat("CREATE INDEX {2}IX_{0}_{1}{3} ON {2}{0}{3} ({2}{1}{3});", WithoutQuotes(HiLoIdentityTableName), WithoutQuotes(TableColumnName), Dialect.OpenQuote, Dialect.CloseQuote);
            script.AppendLine();

            foreach (var entityName in entityNames)
            {
                script.AppendFormat("INSERT INTO {0} ({1}, {2}) VALUES ('[{3}]', 0);", HiLoIdentityTableName, TableColumnName, NextHiValueColumnName, entityName);
                script.AppendLine();
            }
            return script.ToString();
        }
    }
}
