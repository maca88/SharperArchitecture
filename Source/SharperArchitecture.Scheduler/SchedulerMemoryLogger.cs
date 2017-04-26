using System;
using System.Collections.Generic;
using System.Linq;
using BAF.Common.Collections;
using BAF.Scheduler.Entities;

namespace BAF.Scheduler
{
    public class SchedulerMemoryLogger : SchedulerLoggerBase
    {
        private readonly CircularLinkedList<SchedulerLog> _entries;

        public SchedulerMemoryLogger(string schedulerName, int capacity)
            : base(schedulerName)
        {
            _entries = new CircularLinkedList<SchedulerLog>(capacity);
        }

        public override void GetLogs(Action<IQueryable<SchedulerLog>> queryAction)
        {
            var query = _entries.AsQueryable();
            queryAction(query);
        }

        public override void Save(SchedulerLog log)
        {
            _entries.Add(log);
        }
    }
}
