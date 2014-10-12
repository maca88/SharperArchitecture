using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public static class TriggerTypeExtensions
    {
        public static BaseTrigger CreateNewTrigger(this TriggerType triggerType)
        {
            switch (triggerType)
            {
                case TriggerType.Cron:
                    return new CronTrigger();
                case TriggerType.DailyTimeInterval:
                    return new DailyTimeIntervalTrigger();
                case TriggerType.Simple:
                    return new SimpleTrigger();
                default:
                    throw new NotSupportedException(string.Format("not supported triggerType '{0}'", triggerType));
            }
        }

        public static Type GetInstanceType(this TriggerType triggerType)
        {
            switch (triggerType)
            {
                case TriggerType.Cron:
                    return typeof (CronTrigger);
                case TriggerType.DailyTimeInterval:
                    return typeof (DailyTimeIntervalTrigger);
                case TriggerType.Simple:
                    return typeof (SimpleTrigger);
                default:
                    throw new NotSupportedException(string.Format("not supported triggerType '{0}'", triggerType));
            }
        }
    }
}
