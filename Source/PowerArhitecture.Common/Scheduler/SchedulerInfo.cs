using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class SchedulerInfo
    {
        public string Summary { get; set; }

        public string SchedulerName { get; set; }

        public string SchedulerInstanceId { get; set; }

        public Type SchedulerType { get; set; }

        public bool Started { get; set; }

        public bool InStandbyMode { get; set; }

        public bool Shutdown { get; set; }

        public Type JobStoreType { get; set; }

        public Type ThreadPoolType { get; set; }

        public int ThreadPoolSize { get; set; }

        public string Version { get; set; }

        public int NumberOfJobsExecuted { get; set; }

        public bool JobStoreSupportsPersistence { get; set; }

        public bool JobStoreClustered { get; set; }

        public DateTimeOffset? RunningSince { get; set; }

        public SchedulerStatus Status 
        { 
            get
            {
                if (Shutdown)
                    return SchedulerStatus.Shutdown;
                if (Started)
                    return SchedulerStatus.Started;
                if (InStandbyMode)
                    return SchedulerStatus.Standby;
                return SchedulerStatus.Unknown;
            } 
        }
    }
}
