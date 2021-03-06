﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate.AdoNet.Util;
using NHibernate.Cfg;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Tool.hbm2ddl
{
    public class SchemaUpdateExt
    {
        private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (SchemaUpdate));
        private readonly Configuration configuration;
        private readonly IConnectionHelper connectionHelper;
        private readonly Dialect.Dialect dialect;
        private readonly List<Exception> exceptions;
        private readonly IFormatter formatter;

        public SchemaUpdateExt(Configuration cfg) : this(cfg, cfg.Properties)
        {
        }

        public SchemaUpdateExt(Configuration cfg, IDictionary<string, string> configProperties)
        {
            configuration = cfg;
            dialect = Dialect.Dialect.GetDialect(configProperties);
            var props = new Dictionary<string, string>(dialect.DefaultProperties);
            foreach (var prop in configProperties)
            {
                props[prop.Key] = prop.Value;
            }
            connectionHelper = new ManagedProviderConnectionHelper(props);
            exceptions = new List<Exception>();
            formatter =
                (PropertiesHelper.GetBoolean(Environment.FormatSql, configProperties, true)
                     ? FormatStyle.Ddl
                     : FormatStyle.None).Formatter;
        }

        public SchemaUpdateExt(Configuration cfg, Settings settings)
        {
            configuration = cfg;
            dialect = settings.Dialect;
            //connectionHelper = new SuppliedConnectionProviderConnectionHelper(settings.ConnectionProvider);
            exceptions = new List<Exception>();
            formatter = (settings.SqlStatementLogger.FormatSql ? FormatStyle.Ddl : FormatStyle.None).Formatter;
        }

        /// <summary>
        ///     Returns a List of all Exceptions which occured during the export.
        /// </summary>
        /// <returns></returns>
        public IList<Exception> Exceptions
        {
            get { return exceptions; }
        }

        public static void Main(string[] args)
        {
            try
            {
                var cfg = new Configuration();

                bool script = true;
                // If true then execute db updates, otherwise just generate and display updates
                bool doUpdate = true;
                //String propFile = null;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("--"))
                    {
                        if (args[i].Equals("--quiet"))
                        {
                            script = false;
                        }
                        else if (args[i].StartsWith("--properties="))
                        {
                            throw new NotSupportedException("No properties file for .NET, use app.config instead");
                            //propFile = args[i].Substring( 13 );
                        }
                        else if (args[i].StartsWith("--config="))
                        {
                            cfg.Configure(args[i].Substring(9));
                        }
                        else if (args[i].StartsWith("--text"))
                        {
                            doUpdate = false;
                        }
                        else if (args[i].StartsWith("--naming="))
                        {
                            cfg.SetNamingStrategy(
                                (INamingStrategy)
                                Environment.BytecodeProvider.ObjectsFactory.CreateInstance(
                                    ReflectHelper.ClassForName(args[i].Substring(9))));
                        }
                    }
                    else
                    {
                        cfg.AddFile(args[i]);
                    }
                }

                /* NH: No props file for .NET
                 * if ( propFile != null ) {
                    Hashtable props = new Hashtable();
                    props.putAll( cfg.Properties );
                    props.load( new FileInputStream( propFile ) );
                    cfg.SetProperties( props );
                }*/

                new SchemaUpdateExt(cfg).Execute(script, doUpdate);
            }
            catch (Exception e)
            {
                log.Error("Error running schema update", e);
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Execute the schema updates
        /// </summary>
        public void Execute(bool script, bool doUpdate)
        {
            if (script)
            {
                Execute(null, Console.WriteLine, null, doUpdate);
            }
            else
            {
                Execute(null, null, null, doUpdate);
            }
        }

        /// <summary>
        ///     Execute the schema updates
        /// </summary>
        /// <param name="scriptAction">The action to write the each schema line.</param>
        /// <param name="doUpdate">Commit the script to DB</param>
        public void Execute(Action<IDbCommand> beforeExecute, Action<IDbCommand> afterExecute, IDbConnection dbConnection, bool doUpdate,
            Func<System.Type, bool> canUpdateTableFn = null)
        {
            log.Info("Running hbm2ddl schema update");

            string autoKeyWordsImport = PropertiesHelper.GetString(Environment.Hbm2ddlKeyWords, configuration.Properties,
                                                                   "not-defined");
            autoKeyWordsImport = autoKeyWordsImport.ToLowerInvariant();
            if (autoKeyWordsImport == Hbm2DDLKeyWords.AutoQuote)
            {
                SchemaMetadataUpdater.QuoteTableAndColumns(configuration);
            }
            DbConnection connection = dbConnection as DbConnection;
            IDbCommand stmt = null;

            exceptions.Clear();

            try
            {
                DatabaseMetadata meta;
                try
                {
                    log.Info("fetching database metadata");
                    if (connection == null)
                    {
                        connectionHelper.Prepare();
                        connection = connectionHelper.Connection;
                    }
                    meta = new DatabaseMetadata(connection, dialect);
                    stmt = connection.CreateCommand();
                }
                catch (Exception sqle)
                {
                    exceptions.Add(sqle);
                    log.Error("could not get database metadata", sqle);
                    throw;
                }

                log.Info("updating schema");

                string[] createSQL = configuration.GenerateSchemaUpdateScript(dialect, meta, table =>
                {
                    if (canUpdateTableFn == null) return true;
                    var pClass = configuration.ClassMappings.First(o => o.Table.Name == table.Name);
                    return canUpdateTableFn(pClass.MappedClass);
                });
                for (int j = 0; j < createSQL.Length; j++)
                {
                    string sql = createSQL[j];
                    stmt.CommandText = sql;
                    if (beforeExecute != null)
                        beforeExecute(stmt);
                    string formatted = formatter.Format(stmt.CommandText);

                    try
                    {
                        if (doUpdate)
                        {
                            log.Debug(formatted);
                            stmt.ExecuteNonQuery();

                            if (afterExecute != null)
                                afterExecute(stmt);
                        }
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                        log.Error("Unsuccessful: " + sql, e);
                    }
                }

                log.Info("schema update complete");
            }
            catch (Exception e)
            {
                exceptions.Add(e);
                log.Error("could not complete schema update", e);
            }
            finally
            {
                try
                {
                    if (stmt != null)
                    {
                        stmt.Dispose();
                    }
                    connectionHelper.Release();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                    log.Error("Error closing connection", e);
                }
            }
        }
    }
}