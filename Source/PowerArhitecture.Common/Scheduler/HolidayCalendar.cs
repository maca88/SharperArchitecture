using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class HolidayCalendar : BaseCalendar
    {
        public HolidayCalendar()
        {
            ExcludedDates = new List<DateTime>();
            ExcludedDates.Add(DateTime.Now);
            ExcludedDates.Add(DateTime.Now.AddDays(1));
            ExcludedDates.Add(DateTime.Now.AddDays(2));
        }

        public IList<DateTime> ExcludedDates { get; set; }

        public override CalendarType Type
        {
            get { return CalendarType.Holiday; }
        }
    }
}
