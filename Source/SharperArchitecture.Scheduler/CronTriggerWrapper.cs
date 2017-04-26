using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.Scheduler.Specifications;
using Quartz;
using Quartz.Impl.Triggers;

namespace BAF.Scheduler
{
    [Serializable]
    public class CronTriggerWrapper : CronTriggerImpl, IAuditTrigger
    {
        public CronTriggerWrapper(CronTriggerImpl instance)
        {
            CronExpression = new CronExpression(instance.CronExpressionString);
            TimeZone = instance.TimeZone;
            MisfireInstruction = instance.MisfireInstruction;
        }

        public string CreatedBy { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
