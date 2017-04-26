using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Scheduler.Filters
{
    public class JobLogFilter
    {
        public string SchedulerName { get; set; }

        public string JobName { get; set; }

        public string JobGroup { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
