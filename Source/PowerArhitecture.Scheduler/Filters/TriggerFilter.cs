using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;

namespace BAF.Scheduler.Filters
{
    public class TriggerFilter
    {
        public TriggerFilter()
        {
            GroupMatcher = GroupMatcher<TriggerKey>.AnyGroup();

        }

        public string CreatedBy { get; set; }

        public DateTime? DateCreated { get; set; }

        public GroupMatcher<TriggerKey> GroupMatcher { get; set; }
    }
}
