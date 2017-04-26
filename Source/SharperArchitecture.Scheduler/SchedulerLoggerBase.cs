using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BAF.Common.Scheduler;
using BAF.Scheduler.Entities;
using BAF.Scheduler.Specifications;
using Quartz;

namespace BAF.Scheduler
{
    public abstract class SchedulerLoggerBase : ISchedulerLogger
    {
        public SchedulerLoggerBase(string schedulerName)
        {
            SchedulerName = schedulerName;
        }

        public string SchedulerName { get; private set; }

        public abstract void GetLogs(Action<IQueryable<SchedulerLog>> queryAction);

        public abstract void Save(SchedulerLog log);

        string IJobListener.Name { get { return "SchedulerLogger"; } }

        string ITriggerListener.Name { get { return "SchedulerLogger"; } }

        public void JobScheduled(ITrigger trigger)
        {
            var log = Mapper.Map(trigger, CreateLog(SchedulerLogType.JobScheduled));
            Save(log);
        }

        public void JobUnscheduled(TriggerKey triggerKey)
        {
            var log = Mapper.Map(triggerKey, CreateLog(SchedulerLogType.JobUnscheduled));
            Save(log);
        }

        public void TriggerFinalized(ITrigger trigger) 
        {
            var log = Mapper.Map(trigger, CreateLog(SchedulerLogType.TriggerFinalized));
            Save(log);
        }

        public void TriggerPaused(TriggerKey triggerKey)
        {
            var log = Mapper.Map(triggerKey, CreateLog(SchedulerLogType.TriggerPaused));
            Save(log);
        }

        public void TriggersPaused(string triggerGroup)
        {
            var log = CreateLog(SchedulerLogType.TriggersPaused);
            log.TriggerGroup = triggerGroup;
            Save(log);
        }

        public void TriggerResumed(TriggerKey triggerKey)
        {
            var log = Mapper.Map(triggerKey, CreateLog(SchedulerLogType.TriggerResumed));
            Save(log);
        }

        public void TriggersResumed(string triggerGroup)
        {
            var log = CreateLog(SchedulerLogType.TriggersResumed);
            log.TriggerGroup = triggerGroup;
            Save(log);
        }

        public void JobAdded(IJobDetail jobDetail)
        {
            var log = Mapper.Map(jobDetail, CreateLog(SchedulerLogType.JobAdded));
            Save(log);
        }

        public void JobDeleted(JobKey jobKey)
        {
            var log = Mapper.Map(jobKey, CreateLog(SchedulerLogType.JobDeleted));
            Save(log);
        }

        public void JobPaused(JobKey jobKey)
        {
            var log = Mapper.Map(jobKey, CreateLog(SchedulerLogType.JobPaused));
            Save(log);
        }

        public void JobsPaused(string jobGroup)
        {
            var log = CreateLog(SchedulerLogType.JobsPaused);
            log.JobGroup = jobGroup;
            Save(log);
        }

        public void JobResumed(JobKey jobKey)
        {
            var log = Mapper.Map(jobKey, CreateLog(SchedulerLogType.JobResumed));
            Save(log);
        }

        public void JobsResumed(string jobGroup)
        {
            var log = CreateLog(SchedulerLogType.JobsResumed);
            log.JobGroup = jobGroup;
            Save(log);
        }

        public void SchedulerError(string msg, SchedulerException cause) 
        {
            var log = CreateLog(SchedulerLogType.SchedulerError, string.Format("{0} {1} {2}", msg, Environment.NewLine, cause));
            Save(log);
        }

        public void SchedulerInStandbyMode()
        {
            var log = CreateLog(SchedulerLogType.SchedulerInStandbyMode);
            Save(log);
        }

        public void SchedulerStarted()
        {
            var log = CreateLog(SchedulerLogType.SchedulerStarted);
            Save(log);
        }

        public void SchedulerStarting()
        {
            var log = CreateLog(SchedulerLogType.SchedulerStarting);
            Save(log);
        }

        public void SchedulerShutdown() 
        {
            var log = CreateLog(SchedulerLogType.SchedulerShutdown);
            Save(log);
        }

        public void SchedulerShuttingdown()
        {
            var log = CreateLog(SchedulerLogType.SchedulerShuttingdown);
            Save(log);
        }

        public void SchedulingDataCleared()
        {
            var log = CreateLog(SchedulerLogType.SchedulingDataCleared);
            Save(log);
        }

        public void JobToBeExecuted(IJobExecutionContext context) 
        {
            var log = Mapper.Map(context, CreateLog(SchedulerLogType.JobToBeExecuted));
            Save(log);
        }

        public void JobExecutionVetoed(IJobExecutionContext context) 
        {
            var log = Mapper.Map(context, CreateLog(SchedulerLogType.JobVetoed));
            Save(log);
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException) 
        {
            var exception = jobException != null ? jobException.ToString() : null;
            var log = Mapper.Map(context, CreateLog(SchedulerLogType.JobWasExecuted, exception));
            Save(log);
        }

        public void TriggerFired(ITrigger trigger, IJobExecutionContext context) 
        {
            var log = Mapper.Map(context, CreateLog(SchedulerLogType.TriggerFired));
            Save(log);
        }

        public bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context)
        {
            return false;
        }

        public void TriggerMisfired(ITrigger trigger) 
        {
            var log = Mapper.Map(trigger, CreateLog(SchedulerLogType.TriggerMisfired));
            Save(log);
        }

        public void TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode) 
        {
            var log = Mapper.Map(trigger, CreateLog(SchedulerLogType.TriggerComplete));
            Save(log);
        }

        private SchedulerLog CreateLog(SchedulerLogType type, string exception = null)
        {
            return new SchedulerLog
                {
                    Type = type,
                    SchedulerName = SchedulerName,
                    Error = exception
                };
        }
    }
}
