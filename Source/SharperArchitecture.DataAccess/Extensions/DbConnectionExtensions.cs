using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data
{
    public static class DbConnectionExtensions
    {
        public static string MssqlGetUniqueConstraintName(this IDbConnection dbConnection, string tableName, string columnName)
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = @"
                select d.name
                from sys.tables t 
                join sys.indexes d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1
                join sys.index_columns ic on d.index_id=ic.index_id and ic.object_id=t.object_id
                join sys.columns c on ic.column_id = c.column_id  and c.object_id=t.object_id
                where t.name = @table_name and c.name=@col_name";

            var pTableName = command.CreateParameter();
            pTableName.ParameterName = "@table_name";
            pTableName.Value = tableName;
            command.Parameters.Add(pTableName);

            var pColName = command.CreateParameter();
            pColName.ParameterName = "@col_name";
            pColName.Value = columnName;
            command.Parameters.Add(pColName);

            return command.ExecuteScalar() as string;
        }

        public static string MssqlGetPrimaryKeyConstraintName(this IDbConnection dbConnection, string tableName)
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = @"
                select name
                from sys.key_constraints
                where [type] = 'PK'
                and [parent_object_id] = OBJECT_ID(@table_name);";

            var pTableName = command.CreateParameter();
            pTableName.ParameterName = "@table_name";
            pTableName.Value = tableName;
            command.Parameters.Add(pTableName);

            return command.ExecuteScalar() as string;
        }
    }
}
