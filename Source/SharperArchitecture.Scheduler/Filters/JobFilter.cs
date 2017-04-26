using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;

namespace BAF.Scheduler.Filters
{
    public class JobFilter
    {
        public JobFilter()
        {
            GroupMatcher = GroupMatcher<JobKey>.AnyGroup();

        }

        public string CreatedBy { get; set; }

        public GroupMatcher<JobKey> GroupMatcher { get; set; }
    }
}
