using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class CronCalendar : BaseCalendar
    {
        public string CronExpression { get; set; }

        public override CalendarType Type
        {
            get { return CalendarType.Cron; }
        }
    }
}
