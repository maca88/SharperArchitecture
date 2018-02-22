using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Specifications;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;
using SharperArchitecture.Domain.Attributes;

namespace SharperArchitecture.DataAccess.Conventions.Mssql
{
    public class MssqlHiLoIdConvention : BaseHiLoIdConvention
    {
        public MssqlHiLoIdConvention(ConventionsConfiguration conventionsConfiguration) : base(conventionsConfiguration)
        {
        }

        protected override Type GetBaseDialect()
        {
            return typeof(MsSql2000Dialect);
        }

        public override string CreateScript(IEnumerable<string> entityNames)
        {
            var createScript = new StringBuilder();
            createScript.AppendFormat("DELETE FROM {0};", HiLoIdentityTableName);
            createScript.AppendLine();
            createScript.AppendFormat("ALTER TABLE {0} ADD {1} VARCHAR(128) NOT NULL;", HiLoIdentityTableName, TableColumnName);
            createScript.AppendLine();
            createScript.AppendFormat("CREATE NONCLUSTERED INDEX {2}IX_{0}_{1}{3} ON {2}{0}{3} ({2}{1}{3} DESC);", WithoutQuotes(HiLoIdentityTableName), WithoutQuotes(TableColumnName), Dialect.OpenQuote, Dialect.CloseQuote);
            createScript.AppendLine();
            createScript.AppendLine("GO");
            createScript.AppendLine();
            foreach (var entityName in entityNames)
            {
                createScript.AppendFormat("INSERT INTO {0} ({1}, {2}) VALUES ('[{3}]', 0);", HiLoIdentityTableName, TableColumnName, NextHiValueColumnName, entityName);
                createScript.AppendLine();
            }
            return createScript.ToString();
        }
    }
}
