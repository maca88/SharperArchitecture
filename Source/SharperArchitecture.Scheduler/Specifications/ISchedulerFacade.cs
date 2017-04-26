using System;
using System.Collections.Generic;
using System.Linq;
using BAF.Common.Scheduler;
using BAF.Scheduler.Entities;

namespace BAF.Scheduler.Specifications
{
    public interface ISchedulerFacade
    {
        SchedulerInfo GetSchedulerInfo();

        IEnumerable<JobGroup> GetJobGroups(JobGroupFilter filter);

        IEnumerable<Type> GetJobTypes();

        void Save(JobDetail job);

        void PauseJob(string jobName, string jobGroup = null);

        void PauseJobs(string jobGroup = null);

        void ResumeJob(string jobName, string jobGroup = null);

        void ResumeJobs(string jobGroup = null);

        void DeleteJob(string jobName, string groupName = null);

        void ScheduleJob(BaseTrigger cronTrigger, string jobName, string groupName);

        void UnscheduleJob(string triggerName, string triggerGroup);

        void Save(BaseTrigger trigger);

        void TriggerJob(string jobName, string jobGroup = null);

        IEnumerable<JobDetail> GetJobs(JobDetailFilter filter);

        IEnumerable<BaseTrigger> GetTriggers(TriggerDetailFilter filter);

        JobDetail GetJob(string name, string group, bool includeTriggers = false);

        IEnumerable<TriggerGroup> GetTriggerGroups(TriggerGroupFilter filter);

        void PauseTrigger(string name, string group);

        void PauseTrigges(string group);

        IEnumerable<BaseCalendar> GetCalendars();

        BaseCalendar GetCalendar(string name);

        void DeleteCalendar(string name);

        void Save(BaseCalendar calendar);

        void GetLogs(Action<IQueryable<SchedulerLog>> queryAction);
    }
}