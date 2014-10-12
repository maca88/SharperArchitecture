using System;
using System.Collections.Generic;
using System.Linq;
using BAF.Scheduler.Entities;

namespace BAF.Scheduler.Specifications
{
    public interface ILogRepository
    {
        void GetLogs(Action<IQueryable<SchedulerLog>> queryAction);

        void Save(SchedulerLog log);
    }
}