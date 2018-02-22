using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernate.Cfg;
using NHibernate.Dialect;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain.Attributes;

namespace SharperArchitecture.DataAccess.Conventions
{
    public class FormulaAttributeConvention : AttributePropertyConvention<FormulaAttribute>, ISchemaConvention
    {
        protected INamingStrategy NamingStrategy { get; private set; }

        protected Dialect Dialect { get; private set; }

        protected override void Apply(FormulaAttribute attribute, IPropertyInstance instance)
        {
            instance.Formula(Regex.Replace(attribute.SqlFormula, @"`([\w\d]+)`", match =>
                {
                    return NhQuote(NamingStrategy.ColumnName(match.Groups[1].Value));
                },
                RegexOptions.IgnoreCase));
        }

        protected string NhQuote(string name)
        {
            if (!name.StartsWith("`") && !name.EndsWith("`"))
            {
                return $"`{name.TrimStart(Dialect.OpenQuote).TrimEnd(Dialect.CloseQuote)}`";
            }
            return name;
        }

        public bool CanApply(Dialect dialect)
        {
            return true;
        }

        public void Setup(Configuration configuration)
        {
            Dialect = Dialect.GetDialect(configuration.Properties);
            NamingStrategy = configuration.NamingStrategy;
        }

        public void ApplyBeforeExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }

        public void ApplyAfterExecutingQuery(Configuration config, IDbConnection connection, IDbCommand dbCommand)
        {
        }
    }
}
