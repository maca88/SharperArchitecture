using System.Collections.Generic;
using System.Collections.ObjectModel;
using BAF.Scheduler.Filters;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

namespace BAF.Scheduler.Specifications
{
    public interface IBAFJobStore : IJobStore
    {
        Quartz.Collection.ISet<JobKey> GetJobKeys(JobFilter filter);

        Quartz.Collection.ISet<TriggerKey> GetTriggerKeys(TriggerFilter filter);
    }
}