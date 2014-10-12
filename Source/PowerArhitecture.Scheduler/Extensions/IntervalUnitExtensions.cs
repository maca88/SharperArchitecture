using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.Common.Scheduler;

namespace BAF.Scheduler.Extensions
{
    public static class IntervalUnitExtensions
    {
        public static Quartz.IntervalUnit ToQuartz(this IntervalUnit intervalUnit)
        {
            return (Quartz.IntervalUnit) ((int) intervalUnit);
        }

        public static IntervalUnit ToBAF(this  Quartz.IntervalUnit intervalUnit)
        {
            return (IntervalUnit)((int)intervalUnit);
        }
    }
}
