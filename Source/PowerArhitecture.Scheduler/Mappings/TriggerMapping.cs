using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAF.Common.AutoMapper;
using BAF.Common.Scheduler;
using BAF.Common.Specifications;
using BAF.Scheduler.Extensions;
using Quartz;
using Quartz.Impl.Triggers;

namespace BAF.Scheduler.Mappings
{
    public class TriggerMapping: Profile
    {
        private readonly Lazy<IUserCache> _userTask;

        public TriggerMapping(Lazy<IUserCache> userTask)
        {
            _userTask = userTask;
        }

        protected override void Configure()
        {
            CreateMap<BaseTrigger, AbstractTrigger>()
                .Include<SimpleTrigger, SimpleTriggerImpl>()
                .Include<DailyTimeIntervalTrigger, DailyTimeIntervalTriggerImpl>()
                .Include<CronTrigger, CronTriggerImpl>()
                .ForMember(dest => dest.FireInstanceId, o => o.Ignore())
                .ForMember(dest => dest.EndTimeUtc, o => o.MapFrom(src => GetDateTimeOffset(src.EndTimeUtc)))
                .ForMember(dest => dest.StartTimeUtc, o => o.MapFrom(src => GetDateTimeOffset(src.StartTimeUtc)))
                .ForMember(dest => dest.JobDataMap, o => o.MapFrom(src => new JobDataMap(src.GetJobData())))
                .ForMember(dest => dest.JobKey, o => o.MapFrom(src => new JobKey(src.JobName, src.JobGroup)))
                .ForMember(dest => dest.Key, o => o.MapFrom(src => new TriggerKey(src.Name, src.Group)));
            
            CreateMap<SimpleTrigger, SimpleTriggerImpl>()
                .ForMember(dest => dest.TimesTriggered, o => o.Ignore());
            CreateMap<DailyTimeIntervalTrigger, DailyTimeIntervalTriggerImpl>()
                .ForMember(dest => dest.TimeZone, o => o.ResolveUsing<TimeZoneInfoValueResolver>())
                .ForMember(dest => dest.TimesTriggered, o => o.Ignore())
                .ForMember(dest => dest.RepeatIntervalUnit, o => o.MapFrom(src => src.RepeatIntervalUnit.ToQuartz()))
                .ForMember(dest => dest.DaysOfWeek, o => o.MapFrom(src => new Quartz.Collection.HashSet<DayOfWeek>(src.DaysOfWeek)))
                .ForMember(dest => dest.StartTimeOfDay, o => o.MapFrom(src => src.StartTimeOfDay.ToTimeOfDay()))
                .ForMember(dest => dest.EndTimeOfDay, o => o.MapFrom(src => src.EndTimeOfDay.ToTimeOfDay()));
            
            CreateMap<CronTrigger, CronTriggerImpl>()
                .ForMember("CronExpression", o => o.Ignore())
                .ForMember(dest => dest.TimeZone, o => o.ResolveUsing<TimeZoneInfoValueResolver>())
                .ForMember(dest => dest.CronExpressionString, o => o.MapFrom(src => src.CronExpression));
            
            
            CreateMap<AbstractTrigger, BaseTrigger>()
                .Include<SimpleTriggerImpl, SimpleTrigger>()
                .Include<DailyTimeIntervalTriggerImpl, DailyTimeIntervalTrigger>()
                .Include<CronTriggerImpl, CronTrigger>()
                .ForMember(dest => dest.Status, o => o.Ignore())
                .ForMember(dest => dest.EndTimeUtc, o => o.MapFrom(src => GetDateTime(src.EndTimeUtc)))
                .ForMember(dest => dest.FinalFireTimeUtc, o => o.MapFrom(src => GetDateTime(src.FinalFireTimeUtc)))
                .ForMember(dest => dest.NextFireTimeUtc, o => o.MapFrom(src => GetDateTime(src.GetNextFireTimeUtc())))
                .ForMember(dest => dest.PreviousFireTimeUtc, o => o.MapFrom(src => GetDateTime(src.GetPreviousFireTimeUtc())))
                .ForMember(dest => dest.StartTimeUtc, o => o.MapFrom(src => GetDateTime(src.EndTimeUtc)))
                .ForMember(dest => dest.JobData, o => o.MapFrom(src => src.JobDataMap.Select(p => new Parameter { Name = p.Key, Value = p.Value }).ToList()))
                .ForMember(dest => dest.JobName, o => o.MapFrom(src => src.JobKey.Name))
                .ForMember(dest => dest.JobGroup, o => o.MapFrom(src => src.JobKey.Group))
                .ForMember(dest => dest.Name, o => o.MapFrom(src => src.Key.Name))
                .ForMember(dest => dest.Group, o => o.MapFrom(src => src.Key.Group))
                .ForMember(dest => dest.CreatedBy, o => o.MapFrom(src => src.GetCreatedBy()))
                .ForMember(dest => dest.DateCreated, o => o.MapFrom(src => GetDateTime(src.GetDateCreated())));

            CreateMap<SimpleTriggerImpl, SimpleTrigger>();
            CreateMap<DailyTimeIntervalTriggerImpl, DailyTimeIntervalTrigger>()
                .ForMember(dest => dest.RepeatIntervalUnit, o => o.MapFrom(src => src.RepeatIntervalUnit.ToBAF()))
                .ForMember(dest => dest.DaysOfWeek, o => o.MapFrom(src => src.DaysOfWeek.ToList()))
                .ForMember(dest => dest.StartTimeOfDay, o => o.MapFrom(src => src.StartTimeOfDay.ToTimeSpan()))
                .ForMember(dest => dest.EndTimeOfDay, o => o.MapFrom(src => src.EndTimeOfDay.ToTimeSpan()));
            CreateMap<CronTriggerImpl, CronTrigger>()
                .ForMember(dest => dest.CronExpression, o => o.MapFrom(src => src.CronExpressionString));
        }

        private DateTimeOffset? GetDateTimeOffset(DateTime? dateTime)
        {
            return !dateTime.HasValue ? default(DateTimeOffset?) : GetDateTimeOffset(dateTime.Value);
        }

        private DateTimeOffset GetDateTimeOffset(DateTime dateTime)
        {
            return _userTask.Value.GetDateTimeOffset(dateTime);
        }

        private DateTime? GetDateTime(DateTimeOffset? dateTimeOffset)
        {
            return !dateTimeOffset.HasValue ? default(DateTime?) : GetDateTime(dateTimeOffset.Value);
        }

        private DateTime GetDateTime(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.DateTime.Add(dateTimeOffset.Offset);
        }
    }
}
