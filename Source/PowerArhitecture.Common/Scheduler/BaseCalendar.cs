using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public abstract class BaseCalendar
    {
        public string Name { get; set; }

        public TimeZoneInfo TimeZone { get; set; }

        public string Description { get; set; }

        public DateTime NextIncludedTimeUtc { get; set; }

        public abstract CalendarType Type { get; }
    }
}
