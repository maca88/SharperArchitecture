using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace BAF.Scheduler.Extensions
{
    public static class TimeOfDayExtensions
    {
        public static TimeSpan ToTimeSpan(this TimeOfDay timeOfDay)
        {
            return new TimeSpan(timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second);
        }
    }
}
