using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.Scheduler.Specifications;
using Quartz.Impl.Triggers;

namespace BAF.Scheduler
{
    [Serializable]
    public class DailyTimeIntervalTriggerWrapper : DailyTimeIntervalTriggerImpl, IAuditTrigger
    {
        public DailyTimeIntervalTriggerWrapper(DailyTimeIntervalTriggerImpl instance)
        {
            RepeatInterval = instance.RepeatInterval;
            RepeatIntervalUnit = instance.RepeatIntervalUnit;
            MisfireInstruction = instance.MisfireInstruction;
            RepeatCount = instance.RepeatCount;
            TimeZone = instance.TimeZone;
            DaysOfWeek = instance.DaysOfWeek;
            StartTimeOfDay = instance.StartTimeOfDay;
            EndTimeOfDay = instance.EndTimeOfDay;
        }

        public string CreatedBy { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
