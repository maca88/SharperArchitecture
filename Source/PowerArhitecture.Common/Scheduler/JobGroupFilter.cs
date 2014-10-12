using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class JobGroupFilter : JobDetailFilter
    {
        public bool IncludeJobDetails { get; set; }
    }
}
