using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoMapper;
using BAF.Common.Scheduler;
using BAF.Scheduler.Entities;
using BAF.Scheduler.Extensions;
using BAF.Scheduler.Filters;
using BAF.Scheduler.Specifications;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using TriggerGroupFilter = BAF.Common.Scheduler.TriggerGroupFilter;

namespace BAF.Scheduler
{
    public class SchedulerFacade : ISchedulerFacade
    {
        private readonly IBAFScheduler _scheduler;
        private readonly IList<Type> _jobTypes; 

        public SchedulerFacade(IBAFScheduler scheduler)
        {
            _scheduler = scheduler;
            _jobTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(o => o.GetTypes().Where(t => typeof (IJob).IsAssignableFrom(t)))
                         .ToList();
        }

        public SchedulerInfo GetSchedulerInfo()
        {
            return Mapper.Map<SchedulerMetaData, SchedulerInfo>(_scheduler.GetMetaData());
        }

        #region Triggers

        public IEnumerable<BaseTrigger> GetTriggers(TriggerDetailFilter filter)
        {
            filter = filter ?? new TriggerDetailFilter();
            var groupMatcher = GroupMatcher<TriggerKey>.AnyGroup();
            if (!string.IsNullOrEmpty(filter.GroupName))
                groupMatcher = GroupMatcher<TriggerKey>.GroupEquals(filter.GroupName);
            return _scheduler.GetTriggerKeys(new TriggerFilter
            {
                CreatedBy = filter.CreatedBy,
                GroupMatcher = groupMatcher
            }).Select(t => Convert(_scheduler.GetTrigger(t)));
        }

        public IEnumerable<TriggerGroup> GetTriggerGroups(TriggerGroupFilter filter)
        {
            var pausedGroups = _scheduler.GetPausedTriggerGroups();

            return _scheduler.GetTriggerGroupNames().Select(o => new TriggerGroup
            {
                Name = o,
                IsPaused = pausedGroups.Contains(o),
                TriggerDetails = filter.IncludeTriggers 
                    ? _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(o))
                        .Select(t => _scheduler.GetTrigger(t))
                        .Select(Convert)
                    : new List<BaseTrigger>()
            });
        }

        public void Save(BaseTrigger trigger)
        {
            var schTrigger = Mapper.Map(trigger, trigger.GetType(), typeof(AbstractTrigger)) as AbstractTrigger;
            if (schTrigger != null && IsSaveRequired(schTrigger))
            {
                _scheduler.UnscheduleJob(schTrigger.Key);
                _scheduler.ScheduleJob(schTrigger);
            }
        }

        public void PauseTrigger(string name, string group)
        {
            _scheduler.PauseTrigger(new TriggerKey(name, group));
        }

        public void PauseTrigges(string group)
        {
            var groupMatcher = GroupMatcher<TriggerKey>.AnyGroup();
            if (!string.IsNullOrEmpty(group))
                groupMatcher = GroupMatcher<TriggerKey>.GroupEquals(group);
            _scheduler.PauseTriggers(groupMatcher);
        }

        #endregion

        #region Jobs

        public IEnumerable<Type> GetJobTypes()
        {
            return _jobTypes.ToList();
        }

        public void PauseJob(string jobName, string jobGroup = null)
        {
            _scheduler.PauseJob(new JobKey(jobName, jobGroup));
        }

        public void PauseJobs(string jobGroup = null)
        {
            var matcher = GroupMatcher<JobKey>.AnyGroup();
            if (!string.IsNullOrEmpty(jobGroup))
                matcher = GroupMatcher<JobKey>.GroupEquals(jobGroup);
            _scheduler.PauseJobs(matcher);
        }

        public void TriggerJob(string jobName, string jobGroup = null)
        {
            _scheduler.TriggerJob(new JobKey(jobName, jobGroup));
        }

        public void ResumeJob(string jobName, string jobGroup = null)
        {
            _scheduler.ResumeJob(new JobKey(jobName, jobGroup));
        }

        public void ResumeJobs(string jobGroup = null)
        {
            var matcher = GroupMatcher<JobKey>.AnyGroup();
            if (!string.IsNullOrEmpty(jobGroup))
                matcher = GroupMatcher<JobKey>.GroupEquals(jobGroup);
            _scheduler.ResumeJobs(matcher);
        }

        public void Save(JobDetail job)
        {
            var jobDetail = Mapper.Map<JobDetail, IJobDetail>(job);
            if(IsSaveRequired(jobDetail))
                _scheduler.AddJob(jobDetail, true);
            foreach (var trigger in job.Triggers)
            {
                Save(trigger);
            }
        }

        public void DeleteJob(string jobName, string groupName = null)
        {
            _scheduler.DeleteJob(new JobKey(jobName, groupName));
        }

        public void GetLogs(Action<IQueryable<SchedulerLog>> queryAction)
        {
            _scheduler.Logger.GetLogs(queryAction);
        }

        public void ScheduleJob(BaseTrigger trigger, string jobName, string groupName)
        {
            var jobDetails = _scheduler.GetJobDetail(new JobKey(jobName, groupName));
            if(jobDetails == null)
                throw new NullReferenceException("jobDetails");

            var cronTrigger = trigger as CronTrigger;
            if (cronTrigger != null)
            {
                _scheduler.ScheduleJob(jobDetails, BuildTrigger(cronTrigger));
                return;
            }
            var intervalTrigger = trigger as DailyTimeIntervalTrigger;
            if (intervalTrigger != null)
            {
                _scheduler.ScheduleJob(jobDetails, BuildTrigger(intervalTrigger));
                return;
            }
            var simpleTrigger = trigger as SimpleTrigger;
            if (simpleTrigger != null)
            {
                _scheduler.ScheduleJob(jobDetails, BuildTrigger(simpleTrigger));
            }
        }

        public void UnscheduleJob(string triggerName, string triggerGroup)
        {
            _scheduler.UnscheduleJob(new TriggerKey(triggerName, triggerGroup));
        }

        public JobDetail GetJob(string name, string group, bool includeTriggers = false)
        {
            var jobKey = new JobKey(name, group);
            var oldestContext = _scheduler.GetCurrentlyExecutingJobs()
                                          .Where(r => r.JobDetail.Key.ToString() == jobKey.ToString())
                                          .OrderBy(r => r.FireTimeUtc).FirstOrDefault();
            var jobDetail = _scheduler.GetJobDetail(jobKey);
            return Convert(jobDetail, includeTriggers, oldestContext);
        }

        public IEnumerable<JobDetail> GetJobs(JobDetailFilter filter)
        {
            filter = filter ?? new JobDetailFilter();
            var runningJobs = _scheduler.GetCurrentlyExecutingJobs();
            var groupMatcher = GroupMatcher<JobKey>.AnyGroup();
            if (!string.IsNullOrEmpty(filter.GroupName))
                groupMatcher = GroupMatcher<JobKey>.GroupEquals(filter.GroupName);
            var jobKeys = _scheduler.GetJobKeys(new JobFilter
                {
                    CreatedBy = filter.CreatedBy,
                    GroupMatcher = groupMatcher
                });
            return jobKeys
                .Select(o => new
                    {
                        JobDetail = _scheduler.GetJobDetail(o),
                        OldestContext = runningJobs
                            .Where(r => r.JobDetail.Key.ToString() == o.ToString())
                            .OrderBy(r => r.FireTimeUtc).FirstOrDefault()
                    })
                .Select(o => Convert(o.JobDetail, filter.IncludeTriggers, o.OldestContext));
        }

        public IEnumerable<JobGroup> GetJobGroups(JobGroupFilter filter)
        {
            filter = filter ?? new JobGroupFilter();
            var pausedGroups = _scheduler.GetPausedTriggerGroups();
            return _scheduler.GetJobGroupNames().Select(o => new JobGroup
            {
                Name = o,
                IsPaused = pausedGroups.Contains(o),
                JobDetails = filter.IncludeJobDetails ? GetJobs(filter) : new List<JobDetail>()
            });
        }

        #endregion

        #region Calendars

        public IEnumerable<BaseCalendar> GetCalendars()
        {
            return _scheduler.GetCalendarNames()
                .Select(n => Convert(_scheduler.GetCalendar(n), n))
                .ToList();
        }

        public BaseCalendar GetCalendar(string name)
        {
            return Convert(_scheduler.GetCalendar(name), name);
        }

        public void DeleteCalendar(string name)
        {
            _scheduler.DeleteCalendar(name);
        }

        public void Save(BaseCalendar calendar)
        {
            
        }

        #endregion

        #region Private functions

        private ITrigger BuildTrigger(CronTrigger trigger)
        {
            return FillGenericTrigerBuilder(trigger)
                                .WithCronSchedule(trigger.CronExpression, o => o
                                    .WithMisfireHandlingInstruction(trigger.MisfireInstruction))
                                .Build();
        }

        private ITrigger BuildTrigger(DailyTimeIntervalTrigger trigger)
        {
            return FillGenericTrigerBuilder(trigger)
                          .WithDailyTimeIntervalSchedule(o => o
                                .EndingDailyAt(trigger.EndTimeOfDay.ToTimeOfDay())
                                .OnDaysOfTheWeek(!trigger.DaysOfWeek.Any()
                                    ? DailyTimeIntervalScheduleBuilder.AllDaysOfTheWeek
                                    : new Quartz.Collection.HashSet<DayOfWeek>(trigger.DaysOfWeek))
                                .StartingDailyAt(trigger.StartTimeOfDay.ToTimeOfDay())
                                .WithInterval(trigger.RepeatInterval, trigger.RepeatIntervalUnit.ToQuartz())
                                .WithMisfireHandlingInstruction(trigger.MisfireInstruction)
                                .WithRepeatCount(trigger.RepeatCount))
                          .Build();
        }

        private ITrigger BuildTrigger(SimpleTrigger trigger)
        {
            return FillGenericTrigerBuilder(trigger)
                        .WithSimpleSchedule(o => o
                            .WithInterval(trigger.RepeatInterval)
                            .WithRepeatCount(trigger.RepeatCount))
                        .Build();
        }

        private TriggerBuilder FillGenericTrigerBuilder(BaseTrigger trigger)
        {
            return TriggerBuilder.Create()
                                 .WithDescription(trigger.Description)
                                 .WithIdentity(trigger.Name, trigger.Group)
                                 .WithPriority(trigger.Priority)
                                 .EndAt(trigger.EndTimeUtc)
                                 .StartAt(trigger.StartTimeUtc)
                                 .ModifiedByCalendar(trigger.CalendarName)
                                 .UsingJobData(new JobDataMap(trigger.GetJobData()));
        }

        private JobDetail Convert(IJobDetail jobDetail, bool includeTriggers, IJobExecutionContext oldestContext)
        {
            var result = new JobDetail
                {
                    RunningSince = oldestContext != null ? oldestContext.FireTimeUtc : null,
                    Triggers = includeTriggers
                                   ? _scheduler.GetTriggersOfJob(jobDetail.Key)
                                               .Select(Convert)
                                               .ToList()
                                   : new List<BaseTrigger>()
                };
            return Mapper.Map(jobDetail, result);
        }

        private BaseTrigger Convert(ITrigger t)
        {
            var schTrigger = Mapper.Map(t, t.GetType(), typeof(BaseTrigger)) as BaseTrigger;
            if(schTrigger == null)
                throw new NullReferenceException("schTrigger");
            schTrigger.Status = (TriggerStatus)((int)_scheduler.GetTriggerState(t.Key));
            return schTrigger;
        }

        private BaseCalendar Convert(ICalendar calendar, string name)
        {
            var schCalendar = Mapper.Map(calendar, calendar.GetType(), typeof(BaseCalendar)) as BaseCalendar;
            if (schCalendar == null)
                throw new NullReferenceException("schCalendar");
            schCalendar.Name = name;
            return schCalendar;
        }

        private bool IsSaveRequired(IJobDetail jobDetail)
        {
            var toCompare = _scheduler.GetJobDetail(jobDetail.Key);
            if (toCompare == null) return true;

            return 
                !jobDetail.Equals(toCompare, true, true) ||
                !jobDetail.JobDataMap.Count.Equals(toCompare.JobDataMap.Count) ||
                !jobDetail.JobDataMap.All(o => toCompare.JobDataMap.ContainsKey(o.Key) && toCompare.JobDataMap[o.Key] == o.Value) || 
                !(jobDetail.JobType == toCompare.JobType) ||
                !jobDetail.Key.Equals(toCompare.Key);
        }

        private bool IsSaveRequired(ITrigger trigger)
        {
            var toCompare = _scheduler.GetTrigger(trigger.Key);
            if (toCompare == null) return true;

            return
                !trigger.Equals(toCompare, true, true) ||
                !trigger.JobDataMap.Count.Equals(toCompare.JobDataMap.Count) ||
                !trigger.JobDataMap.All(o => toCompare.JobDataMap.ContainsKey(o.Key) && toCompare.JobDataMap[o.Key] == o.Value) ||
                !trigger.JobKey.Equals(toCompare.JobKey) ||
                !trigger.Key.Equals(toCompare.Key) ||
                IsSaveRequired(trigger as IDailyTimeIntervalTrigger, toCompare as IDailyTimeIntervalTrigger) ||
                IsSaveRequired(trigger as ICronTrigger, toCompare as ICronTrigger);
        }

        private static bool IsSaveRequired(IDailyTimeIntervalTrigger trigger, IDailyTimeIntervalTrigger toCompare)
        {
            return !(
                trigger == null || 
                toCompare == null || 
                (
                    trigger.DaysOfWeek.Count == toCompare.DaysOfWeek.Count &&
                    trigger.DaysOfWeek.All(o => toCompare.DaysOfWeek.Contains(o))
                )
            );
        }

        private static bool IsSaveRequired(ICronTrigger trigger, ICronTrigger toCompare)
        {
            return !(
                trigger == null ||
                toCompare == null ||
                trigger.CronExpressionString == toCompare.CronExpressionString
            );
        }

        #endregion
    }
}
