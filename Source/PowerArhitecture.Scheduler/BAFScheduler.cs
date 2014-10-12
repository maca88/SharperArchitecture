using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BAF.Scheduler.Filters;
using BAF.Scheduler.Specifications;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace BAF.Scheduler
{
    public class BAFScheduler : StdScheduler, IBAFScheduler
    {
        private ISchedulerLogger _logger;
        private readonly IResolutionRoot _resolutionRoot;
        private readonly QuartzScheduler _scheduler;
        private readonly IBAFJobStore _bafJobStore;
        private readonly MethodInfo _notifySchedulerThreadMethodInfo;

        public BAFScheduler(IResolutionRoot resolutionRoot, QuartzScheduler scheduler, QuartzSchedulerResources resources) : base(scheduler)
        {
            _resolutionRoot = resolutionRoot;
            _scheduler = scheduler;
            _bafJobStore = resources.JobStore as IBAFJobStore;
            _notifySchedulerThreadMethodInfo = typeof(QuartzScheduler).GetMethod("NotifySchedulerThread", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static readonly string DefaultSchedulerName = "Scheduler";

        public virtual Quartz.Collection.ISet<JobKey> GetJobKeys(JobFilter filter)
        {
            _scheduler.ValidateState();
            return _bafJobStore.GetJobKeys(filter);
        }

        public Quartz.Collection.ISet<TriggerKey> GetTriggerKeys(TriggerFilter filter)
        {
            _scheduler.ValidateState();
            return _bafJobStore.GetTriggerKeys(filter);
        }

        public ISchedulerLogger Logger
        {
            get { return _logger ?? (_logger = CreateLogger()); }
            set
            {
                DisposeLogger(_logger);
                _logger = value;
            }
        }

        #region Triggers


        #endregion

        private ISchedulerLogger CreateLogger()
        {
            var logger = _resolutionRoot.Get<ISchedulerLogger>(new IParameter[]
            {
                new ConstructorArgument("schedulerName", SchedulerName),
                new ConstructorArgument("capacity", 1000)
            });
            ListenerManager.AddJobListener(logger);
            ListenerManager.AddTriggerListener(logger);
            ListenerManager.AddSchedulerListener(logger);
            return logger;
        }

        private void DisposeLogger(ISchedulerLogger logger)
        {
            ListenerManager.RemoveJobListener(((IJobListener) logger).Name);
            ListenerManager.RemoveTriggerListener(((ITriggerListener)logger).Name);
            ListenerManager.RemoveSchedulerListener(logger);
        }

        private void NotifySchedulerThread(DateTimeOffset? candidateNewNextFireTimeUtc)
        {
            _notifySchedulerThreadMethodInfo.Invoke(_scheduler, new object[] {candidateNewNextFireTimeUtc});
        }
    }
}
