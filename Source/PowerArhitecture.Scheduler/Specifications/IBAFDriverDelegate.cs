using System;
using System.Collections.Generic;
using BAF.Scheduler.Filters;
using Quartz;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.Matchers;

namespace BAF.Scheduler.Specifications
{
    public interface IBAFDriverDelegate : IDriverDelegate
    {
        Quartz.Collection.ISet<JobKey> SelectJobs(ConnectionAndTransactionHolder conn, JobFilter filter);

        Quartz.Collection.ISet<TriggerKey> SelectTriggers(ConnectionAndTransactionHolder conn, TriggerFilter filter);

    }
}