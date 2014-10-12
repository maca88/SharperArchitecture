using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class TriggerGroup
    {
        public TriggerGroup()
        {
            TriggerDetails = new List<BaseTrigger>();
        }

        public string Name { get; set; }

        public bool IsPaused { get; set; }

        public IEnumerable<BaseTrigger> TriggerDetails { get; set; }  
    }
}
