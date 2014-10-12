using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class JobGroup
    {
        public JobGroup()
        {
            JobDetails = new List<JobDetail>();
        }

        public string Name { get; set; }

        public bool IsPaused { get; set; }

        public IEnumerable<JobDetail> JobDetails { get; set; } 
    }
}
