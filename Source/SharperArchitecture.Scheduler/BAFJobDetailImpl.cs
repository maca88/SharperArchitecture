using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz.Impl;

namespace BAF.Scheduler
{
    [Serializable]
    public class BAFJobDetailImpl : JobDetailImpl
    {
        public string CreatedBy { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
