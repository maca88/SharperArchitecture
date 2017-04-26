using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;
using BAF.Libraries.Common.Settings;
using BAF.Libraries.DataAccess.Specifications;

namespace BAF.Libraries.DataAccess.Conventions.Mssql
{
    //This will create a foreign key on a one to one association with a foreign id
    public class MssqlForeignIdConvention : ICreateSchemaConvention
    {
        private readonly ConventionsSettings _settings;
         private readonly HashSet<string> _validDialects = new HashSet<string>
            {
                typeof (MsSql2000Dialect).FullName,
                typeof (MsSql2005Dialect).FullName,
                typeof (MsSql2008Dialect).FullName
            };

         public MssqlForeignIdConvention(ConventionsSettings conventionsSettings)
        {
            _settings = conventionsSettings;

        }

        public bool CanApply(Dialect dialect)
        {
            return _validDialects.Contains(dialect.GetType().FullName);
        }

        public void ApplyBeforeExecutingQuery(Configuration config, IDbConnection connection, string sql)
        {
        }

        public void ApplyAfterExecutingQuery(Configuration config, IDbConnection connection, string sql)
        {
        }

        public void ApplyBeforeSchemaCreate(Configuration config, IDbConnection connection)
        {
            return;
            foreach (var cls in config.ClassMappings)
            {
                var columns = cls.IdentifierProperty.ColumnIterator.OfType<Column>().ToList();
                if(columns.Count > 1) continue; //Not supported
                var col = columns.First();
                var colValue = col.Value as SimpleValue;
                if (colValue == null || colValue.IdentifierGeneratorStrategy != "foreign") continue;
                var foreignPropName = colValue.IdentifierGeneratorProperties.First(o => o.Key == "property").Value;
                var foreignProp = cls.PropertyIterator.FirstOrDefault(o => o.Name == foreignPropName);
                if(foreignProp == null) continue;
                var oneToOne = foreignProp.Value as OneToOne;
                if (oneToOne == null) continue;
                var mstCls = config.ClassMappings.FirstOrDefault(o => o.RootClazz.EntityName == oneToOne.Type.ReturnedClass.FullName);
                if(mstCls == null) continue;

                var mstColumns = mstCls.IdentifierProperty.ColumnIterator.OfType<Column>().ToList();
                if (mstColumns.Count > 1) continue; //Not supported
                var mstCol = mstColumns.First();

                var createScript = new StringBuilder();
                createScript.AppendFormat("ALTER TABLE {0} ADD CONSTRAINT FK_{0}_{1} FOREIGN KEY({2}) REFERENCES {1} ({3});",
                    cls.Table.Name, mstCls.Table.Name, col.Name, mstCol.Name);
                createScript.AppendLine();

                var dropScript = new StringBuilder();
                dropScript.AppendFormat("ALTER TABLE {0} DROP CONSTRAINT FK_{0}_{1};",
                    cls.Table.Name, mstCls.Table.Name);
                dropScript.AppendLine();

                config.AddAuxiliaryDatabaseObject(new SimpleAuxiliaryDatabaseObject(createScript.ToString(), null, _validDialects));

            }
        }

        public void ApplyAfterSchemaCreate(Configuration config, IDbConnection connection)
        {
        }
    }
}
