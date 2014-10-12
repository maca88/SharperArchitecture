using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class CronTrigger : BaseTrigger
    {
        public string CronExpression { get; set; }

        public override TriggerType Type
        {
            get { return TriggerType.Cron; }
        }
    }
}
