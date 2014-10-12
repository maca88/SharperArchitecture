using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class SimpleTrigger : BaseTrigger
    {
        public int RepeatCount { get; set; }

        public TimeSpan RepeatInterval { get; set; }

        public override TriggerType Type
        {
            get { return TriggerType.Simple; }
        }
    }
}
