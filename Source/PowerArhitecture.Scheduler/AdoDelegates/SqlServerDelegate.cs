using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Scheduler.AdoDelegates
{
    /// <summary>
    /// A SQL Server specific driver delegate.
    /// </summary>
    /// <author>Marko Lahma</author>
    public class SqlServerDelegate : BAFAdoDelegate
    {
        /// <summary>
        /// Gets the select next trigger to acquire SQL clause.
        /// SQL Server specific version with TOP functionality
        /// </summary>
        /// <returns></returns>
        protected override string GetSelectNextTriggerToAcquireSql(int maxCount)
        {
            string sqlSelectNextTriggerToAcquire = SqlSelectNextTriggerToAcquire;

            // add limit clause to correct place
            sqlSelectNextTriggerToAcquire = "SELECT TOP " + maxCount + " " + sqlSelectNextTriggerToAcquire.Substring(6);

            return sqlSelectNextTriggerToAcquire;
        }

        public override void AddCommandParameter(IDbCommand cmd, string paramName, object paramValue, Enum dataType)
        {
            // deeded for SQL Server CE
            if (paramValue is bool && dataType == default(Enum))
            {
                paramValue = (bool)paramValue ? 1 : 0;
            }

            base.AddCommandParameter(cmd, paramName, paramValue, dataType);
        }
    }
}
