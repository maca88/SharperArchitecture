using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.Common.Attributes;
using BAF.Common.Scheduler;
using BAF.Domain;
using BAF.Validation.Attributes;
using Quartz;

namespace BAF.Scheduler.Entities
{
    public class SchedulerLog : Entity
    {
        public SchedulerLog()
        {
            Date = DateTime.UtcNow;
        }

        [EnumString]
        [NotNull]
        [Length(20)]
        public virtual SchedulerLogType Type { get; set; }

        public virtual string SchedulerName { get; set; }

        public virtual string JobName { get; set; }

        public virtual string JobGroup { get; set; }

        public virtual string TriggerName { get; set; }

        public virtual string TriggerGroup { get; set; }

        public virtual DateTime Date { get; protected set; }

        public virtual string Error { get; set; }
    }
}
