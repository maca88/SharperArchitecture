using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Dialect.Schema;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate
{
    public static class ConfigurationExtensions
    {
        public static string[] GenerateSchemaUpdateScript(this Configuration cfg, Dialect.Dialect dialect, DatabaseMetadata databaseMetadata, Func<Table, bool> canUpdateTableFn)
        {
            var type = typeof(Configuration);
            type.GetMethod("SecondPassCompile", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(cfg, null);
            //SecondPassCompile();
            var properties = type.GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(cfg) as IDictionary<string, string>;
            var TableMappings = type.GetProperty("TableMappings", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod(true).Invoke(cfg, null) as ICollection<Table>;
            var mapping = type.GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(cfg) as IMapping;
            Func<Dialect.Dialect, IEnumerable<IPersistentIdentifierGenerator>> IterateGenerators = (d) =>
            {
                return type.GetMethod("IterateGenerators", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(cfg, new object[] { d }) as IEnumerable<IPersistentIdentifierGenerator>;
            };

            string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
            string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);

            var script = new List<string>(50);
            foreach (var table in TableMappings)
            {
                if (canUpdateTableFn != null && !canUpdateTableFn(table)) continue;

                if (table.IsPhysicalTable && Configuration.IncludeAction(table.SchemaActions, SchemaAction.Update))
                {
                    ITableMetadata tableInfo = databaseMetadata.GetTableMetadata(table.Name, table.Schema ?? defaultSchema,
                                                                                 table.Catalog ?? defaultCatalog, table.IsQuoted);
                    if (tableInfo == null)
                    {
                        script.Add(table.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
                    }
                    else
                    {
                        string[] alterDDL = table.SqlAlterStrings(dialect, mapping, tableInfo, defaultCatalog, defaultSchema);
                        script.AddRange(alterDDL);
                    }

                    string[] comments = table.SqlCommentStrings(dialect, defaultCatalog, defaultSchema);
                    script.AddRange(comments);
                }
            }

            foreach (var table in TableMappings)
            {
                if (canUpdateTableFn != null && !canUpdateTableFn(table)) continue;

                if (table.IsPhysicalTable && Configuration.IncludeAction(table.SchemaActions, SchemaAction.Update))
                {
                    ITableMetadata tableInfo = databaseMetadata.GetTableMetadata(table.Name, table.Schema, table.Catalog,
                                                                                 table.IsQuoted);

                    if (dialect.SupportsForeignKeyConstraintInAlterTable)
                    {
                        foreach (var fk in table.ForeignKeyIterator)
                        {
                            if (fk.HasPhysicalConstraint && Configuration.IncludeAction(fk.ReferencedTable.SchemaActions, SchemaAction.Update))
                            {
                                bool create = tableInfo == null
                                              ||
                                              (tableInfo.GetForeignKeyMetadata(fk.Name) == null
                                               && (!(dialect is MySQLDialect) || tableInfo.GetIndexMetadata(fk.Name) == null));
                                if (create)
                                {
                                    script.Add(fk.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
                                }
                            }
                        }
                    }

                    foreach (var index in table.IndexIterator)
                    {
                        if (tableInfo == null || tableInfo.GetIndexMetadata(index.Name) == null)
                        {
                            script.Add(index.SqlCreateString(dialect, mapping, defaultCatalog, defaultSchema));
                        }
                    }
                }
            }

            foreach (var generator in IterateGenerators(dialect))
            {
                string key = generator.GeneratorKey();
                if (!databaseMetadata.IsSequence(key) && !databaseMetadata.IsTable(key))
                {
                    string[] lines = generator.SqlCreateStrings(dialect);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        script.Add(lines[i]);
                    }
                }
            }

            return script.ToArray();
        }

    }
}
