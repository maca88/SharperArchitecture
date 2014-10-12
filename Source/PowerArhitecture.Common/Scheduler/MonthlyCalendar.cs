using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class MonthlyCalendar : BaseCalendar
    {
        public MonthlyCalendar()
        {
            ExcludedDays = new List<int>();
        }

        public IList<int> ExcludedDays { get; set; }

        public override CalendarType Type
        {
            get { return CalendarType.Monthly; }
        }
    }
}
