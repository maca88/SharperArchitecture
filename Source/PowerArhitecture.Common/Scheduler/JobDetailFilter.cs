using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class JobDetailFilter
    {
        public string GroupName { get; set; }

        public string CreatedBy { get; set; }

        public bool IncludeTriggers { get; set; }
    }
}
