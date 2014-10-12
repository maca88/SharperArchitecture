using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace BAF.Common.Scheduler
{
    public class DailyTimeIntervalTrigger : BaseTrigger
    {
        public DailyTimeIntervalTrigger()
        {
            DaysOfWeek = new List<DayOfWeek>();
        }

        public IntervalUnit RepeatIntervalUnit { get; set; }

        public int RepeatCount { get; set; }

        public int RepeatInterval { get; set; }

        public IList<DayOfWeek> DaysOfWeek { get; set; }

        public TimeSpan StartTimeOfDay { get; set; }

        public TimeSpan EndTimeOfDay { get; set; }

        public override TriggerType Type
        {
            get { return TriggerType.DailyTimeInterval; }
        }
    }
}
