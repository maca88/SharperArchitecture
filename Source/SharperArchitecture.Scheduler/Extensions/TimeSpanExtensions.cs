using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace BAF.Scheduler.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeOfDay ToTimeOfDay(this TimeSpan timeSpan)
        {
            return new TimeOfDay(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}
