using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Scheduler.AdoDelegates
{
    /// <summary>
    /// This is a driver delegate for the Oracle database.
    /// </summary>
    /// <author>Marko Lahma</author>
    public class OracleDelegate : BAFAdoDelegate
    {
        /// <summary>
        /// Creates the SQL for select next trigger to acquire.
        /// </summary>
        protected override string GetSelectNextTriggerToAcquireSql(int maxCount)
        {
            string sqlSelectNextTriggerToAcquire = SqlSelectNextTriggerToAcquire;

            int whereEndIdx = sqlSelectNextTriggerToAcquire.IndexOf("WHERE") + 6;
            string beginningAndWhere = sqlSelectNextTriggerToAcquire.Substring(0, whereEndIdx);
            string theRest = sqlSelectNextTriggerToAcquire.Substring(whereEndIdx);

            // add limit clause to correct place
            return beginningAndWhere + " rownum <= " + maxCount + " AND " + theRest;
        }

        /// <summary>
        /// Gets the db presentation for boolean value. For Oracle we use true/false of "1"/"0".
        /// </summary>
        /// <param name="booleanValue">Value to map to database.</param>
        /// <returns></returns>
        public override object GetDbBooleanValue(bool booleanValue)
        {
            return booleanValue ? "1" : "0";
        }

        public override bool GetBooleanFromDbValue(object columnValue)
        {
            // we store things as string in oracle with 1/0 as value
            if (columnValue != null && columnValue != DBNull.Value)
            {
                return Convert.ToInt32(columnValue) == 1;
            }

            throw new ArgumentException("Value must be non-null.");
        }
    }
}
