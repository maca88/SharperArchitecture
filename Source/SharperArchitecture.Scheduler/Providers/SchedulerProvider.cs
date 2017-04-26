using System;
using System.Collections.Generic;
using BAF.Scheduler.Specifications;
using Ninject.Activation;
using Quartz;
using Quartz.Spi;

namespace BAF.Scheduler.Providers
{
    public class SchedulerProvider : IProvider<IBAFScheduler>
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<IJobListener> _jobListeners;
        private readonly IEnumerable<ISchedulerListener> _schedulerListeners;
        private readonly IEnumerable<ITriggerListener> _triggerListeners;

        public SchedulerProvider(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, 
            IEnumerable<IJobListener> jobListeners, 
            IEnumerable<ISchedulerListener> schedulerListeners,
            IEnumerable<ITriggerListener> triggerListeners)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _jobListeners = jobListeners;
            _schedulerListeners = schedulerListeners;
            _triggerListeners = triggerListeners;
        }

        public object Create(IContext context)
        {
            var scheduler = _schedulerFactory.GetScheduler(BAFScheduler.DefaultSchedulerName) ?? _schedulerFactory.GetScheduler();
            scheduler.JobFactory = _jobFactory;
            foreach (var listener in _jobListeners)
            {
                scheduler.ListenerManager.AddJobListener(listener);
            }
            foreach (var listener in _schedulerListeners)
            {
                scheduler.ListenerManager.AddSchedulerListener(listener);
            }
            foreach (var listener in _triggerListeners)
            {
                scheduler.ListenerManager.AddTriggerListener(listener);
            }
            Type = scheduler.GetType();
            return scheduler;
        }

        public Type Type { get; private set; }
    }
}
