using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Syntax;
using Quartz;
using Quartz.Core;
using Quartz.Impl;

namespace BAF.Scheduler.Factories
{
    public class SchedulerFactory : StdSchedulerFactory
    {
        private readonly IResolutionRoot _resolutionRoot;

        public SchedulerFactory(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            return new BAFScheduler(_resolutionRoot, qs, rsrcs);
        }
    }
}
