using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class AnnualCalendar : BaseCalendar
    {
        public AnnualCalendar()
        {
            DaysExcluded = new List<DateTime>();
        }

        public IList<DateTime> DaysExcluded { get; set; } 

        public override CalendarType Type
        {
            get { return CalendarType.Annual; }
        }
    }
}
