using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public enum TriggerStatus
    {
        Normal,
        Paused,
        Complete,
        Error,
        Blocked,
        None,
    }
}
