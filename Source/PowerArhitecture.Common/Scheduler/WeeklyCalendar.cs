using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class WeeklyCalendar : BaseCalendar
    {
        public WeeklyCalendar()
        {
            ExcludedDaysOfWeek = new List<DayOfWeek>();
        }

        public IList<DayOfWeek> ExcludedDaysOfWeek { get; set; }

        public override CalendarType Type
        {
            get { return CalendarType.Weekly; }
        }
    }
}
