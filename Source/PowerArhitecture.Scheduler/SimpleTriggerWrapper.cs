using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.Scheduler.Specifications;
using Quartz.Impl.Triggers;

namespace BAF.Scheduler
{
    [Serializable]
    public class SimpleTriggerWrapper : SimpleTriggerImpl, IAuditTrigger
    {
        public SimpleTriggerWrapper(SimpleTriggerImpl instance)
        {
            RepeatInterval = instance.RepeatInterval;
            RepeatCount = instance.RepeatCount;
            MisfireInstruction = instance.MisfireInstruction;
        }

        public string CreatedBy { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
