using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Scheduler.AdoDelegates
{
    /// <summary>
    /// This is a driver delegate for the MySQL ADO.NET driver.
    /// </summary>
    /// <author>Marko Lahma</author>
    public class MySQLDelegate : BAFAdoDelegate
    {
        /// <summary>
        /// Gets the select next trigger to acquire SQL clause.
        /// MySQL version with LIMIT support.
        /// </summary>
        /// <returns></returns>
        protected override string GetSelectNextTriggerToAcquireSql(int maxCount)
        {
            return SqlSelectNextTriggerToAcquire + " LIMIT " + maxCount;
        }
    }
}
