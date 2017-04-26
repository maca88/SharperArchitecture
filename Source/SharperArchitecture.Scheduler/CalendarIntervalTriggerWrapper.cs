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
    public class CalendarIntervalTriggerWrapper : CalendarIntervalTriggerImpl, IAuditTrigger
    {
        public CalendarIntervalTriggerWrapper(CalendarIntervalTriggerImpl instance)
        {
            RepeatInterval = instance.RepeatInterval;
            RepeatIntervalUnit = instance.RepeatIntervalUnit;
            MisfireInstruction = instance.MisfireInstruction;
            TimeZone = instance.TimeZone;
            PreserveHourOfDayAcrossDaylightSavings = instance.PreserveHourOfDayAcrossDaylightSavings;
            SkipDayIfHourDoesNotExist = instance.SkipDayIfHourDoesNotExist;
        }

        public string CreatedBy { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    } 
}
