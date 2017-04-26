using System.Collections.Generic;
using BAF.Scheduler.Filters;
using Quartz;
using Quartz.Impl.Matchers;

namespace BAF.Scheduler.Specifications
{
    public interface IBAFScheduler : IScheduler
    {
        ISchedulerLogger Logger { get; set; }

        Quartz.Collection.ISet<JobKey> GetJobKeys(JobFilter filter);

        Quartz.Collection.ISet<TriggerKey> GetTriggerKeys(TriggerFilter filter);
    }
}