using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class DailyCalendar : BaseCalendar
    {
        public TimeSpan ExcludedStartTime { get; set; }

        public TimeSpan ExcludedEndTime { get; set; }

        public bool InvertTimeRange { get; set; }

        public override CalendarType Type
        {
            get { return CalendarType.Daily; }
        }
    }
}
