using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Util;

namespace SharperArchitecture.DataAccess.Internal
{
    internal class CustomAuxiliaryDatabaseObject : IAuxiliaryDatabaseObject
    {
        private readonly string _sqlCreateString;
        private readonly string _sqlDropString;
        private readonly Type _baseDialect;

        public CustomAuxiliaryDatabaseObject(string sqlCreateString, string sqlDropString, Type baseDialect)
        {
            _sqlCreateString = sqlCreateString;
            _sqlDropString = sqlDropString;
            _baseDialect = baseDialect;
        }

        public string SqlCreateString(Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
        {
            return InjectCatalogAndSchema(_sqlCreateString, defaultCatalog, defaultSchema);
        }

        public string SqlDropString(Dialect dialect, string defaultCatalog, string defaultSchema)
        {
            return InjectCatalogAndSchema(_sqlDropString, defaultCatalog, defaultSchema);
        }

        private static string InjectCatalogAndSchema(string ddlString, string defaultCatalog, string defaultSchema)
        {
            return StringHelper.Replace(StringHelper.Replace(ddlString, "${catalog}", defaultCatalog), "${schema}", defaultSchema);
        }

        public void AddDialectScope(string dialectName)
        {
            throw new NotSupportedException();
        }

        public bool AppliesToDialect(Dialect dialect)
        {
            return _baseDialect.IsInstanceOfType(dialect);
        }

        public void SetParameterValues(IDictionary<string, string> parameters)
        {
            throw new NotSupportedException();
        }
    }
}
