using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.Scheduler.Filters;
using BAF.Scheduler.Specifications;
using Quartz;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.Matchers;

namespace BAF.Scheduler
{
    public class BAFJobStoreTX : JobStoreTX, IBAFJobStore
    {
        protected IBAFDriverDelegate BAFDelegate { get { return Delegate as IBAFDriverDelegate; } }

        #region GetJobKeys

        public Quartz.Collection.ISet<JobKey> GetJobKeys(JobFilter filter)
        {
            return ExecuteWithoutLock(conn => GetJobNames(conn, filter));
        }
        
        protected virtual Quartz.Collection.ISet<JobKey> GetJobNames(ConnectionAndTransactionHolder conn, JobFilter filter)
        {
            try
            {
                return BAFDelegate.SelectJobs(conn, filter);
            }
            catch (Exception e)
            {
                throw new JobPersistenceException("Couldn't obtain job names: " + e.Message, e);
            }
        }

        #endregion

        #region GetTriggerKeys

        public Quartz.Collection.ISet<TriggerKey> GetTriggerKeys(TriggerFilter filter)
        {
            return ExecuteWithoutLock(conn => GetTriggerNames(conn, filter));
        }

        protected virtual Quartz.Collection.ISet<TriggerKey> GetTriggerNames(ConnectionAndTransactionHolder conn, TriggerFilter filter)
        {
            try
            {
                return BAFDelegate.SelectTriggers(conn, filter);
            }
            catch (Exception e)
            {
                throw new JobPersistenceException("Couldn't obtain trigger names: " + e.Message, e);
            }
        }

        #endregion
    }
}
