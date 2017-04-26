using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BAF.Common.Specifications;
using BAF.Scheduler.Extensions;
using Quartz;
using Quartz.Impl.Calendar;
using BAFDailyCalendar = BAF.Common.Scheduler.DailyCalendar;
using BAFWeeklyCalendar = BAF.Common.Scheduler.WeeklyCalendar;
using BAFMonthlyCalendar = BAF.Common.Scheduler.MonthlyCalendar;
using BAFAnnualCalendar = BAF.Common.Scheduler.AnnualCalendar;
using BAFHolidayCalendar = BAF.Common.Scheduler.HolidayCalendar;
using BAFCronCalendar = BAF.Common.Scheduler.CronCalendar;
using BAFBaseCalendar = BAF.Common.Scheduler.BaseCalendar;

namespace BAF.Scheduler.Mappings
{
    public class CalendarMapping : Profile
    {
        private readonly Lazy<IUserCache> _userTask;

        public CalendarMapping(Lazy<IUserCache> userTask)
        {
            _userTask = userTask;
        }

        protected override void Configure()
        {
            //From BAF to Quartz
            CreateMap<BAFBaseCalendar, BaseCalendar>()
                .Include<BAFDailyCalendar, DailyCalendar>()
                .Include<BAFWeeklyCalendar, WeeklyCalendar>()
                .Include<BAFMonthlyCalendar, MonthlyCalendar>()
                .Include<BAFAnnualCalendar, AnnualCalendar>()
                .Include<BAFHolidayCalendar, HolidayCalendar>()
                .Include<BAFCronCalendar, CronCalendar>()
                .ForMember(dest => dest.CalendarBase, o => o.Ignore());

            CreateMap<BAFDailyCalendar, DailyCalendar>()
                .ConstructUsing(calendar =>
                {
                    var start = calendar.ExcludedStartTime;
                    var end = calendar.ExcludedEndTime;
                    return new DailyCalendar(
                        start.Hours, start.Minutes, start.Seconds, start.Milliseconds,
                        end.Hours, end.Minutes, end.Seconds, end.Milliseconds
                        );
                });

            CreateMap<BAFWeeklyCalendar, WeeklyCalendar>()
                .ForMember(dest => dest.DaysExcluded, o => o.MapFrom(src => GetExcludedDaysOfweek(src.ExcludedDaysOfWeek)));
            CreateMap<BAFMonthlyCalendar, MonthlyCalendar>()
                .ForMember(dest => dest.DaysExcluded, o => o.MapFrom(src => GetExcludedDaysOfMonth(src.ExcludedDays)));
            CreateMap<BAFAnnualCalendar, AnnualCalendar>()
                .ForMember(dest => dest.DaysExcluded, o => o.MapFrom(src => GetExcludedDays(src.DaysExcluded)));
            CreateMap<BAFHolidayCalendar, HolidayCalendar>()
                .ForMember(dest => dest.ExcludedDates, o => o.MapFrom(src => new SortedSet<DateTime>(src.ExcludedDates)));
            CreateMap<BAFCronCalendar, CronCalendar>()
                .ForMember(dest => dest.CronExpression, o => o.MapFrom(src => new CronExpression(src.CronExpression)));

            //From Quartz to BAF
            CreateMap<BaseCalendar, BAFBaseCalendar>()
                .Include<DailyCalendar, BAFDailyCalendar>()
                .Include<WeeklyCalendar, BAFWeeklyCalendar>()
                .Include<MonthlyCalendar, BAFMonthlyCalendar>()
                .Include<AnnualCalendar, BAFAnnualCalendar>()
                .Include<HolidayCalendar, BAFHolidayCalendar>()
                .Include<CronCalendar, BAFCronCalendar>()
                .ForMember(dest => dest.Name, o => o.Ignore())
                .ForMember(dest => dest.NextIncludedTimeUtc, o => o.MapFrom(src => src.GetNextIncludedTimeUtc(CurrentUserDateTimeOffset()).DateTime));

            CreateMap<DailyCalendar, BAFDailyCalendar>()
                .ForMember(dest => dest.ExcludedStartTime, o => o.MapFrom(src => src.GetExcludedStartTime()))
                .ForMember(dest => dest.ExcludedEndTime, o => o.MapFrom(src => src.GetExcludedEndTime()))
                .ForMember(dest => dest.InvertTimeRange, o => o.MapFrom(src => src.GetInvertTimeRange()));
            CreateMap<WeeklyCalendar, BAFWeeklyCalendar>()
                .ForMember(dest => dest.ExcludedDaysOfWeek, o => o.MapFrom(src => GetExcludedDaysOfWeek(src.DaysExcluded)));
            CreateMap<MonthlyCalendar, BAFMonthlyCalendar>()
                .ForMember(dest => dest.ExcludedDays, o => o.MapFrom(src => GetExcludedDaysOfMonth(src.DaysExcluded)));
            CreateMap<AnnualCalendar, BAFAnnualCalendar>()
                .ForMember(dest => dest.DaysExcluded, o => o.MapFrom(src => GetExcludedDays(src.DaysExcluded)));
            CreateMap<HolidayCalendar, BAFHolidayCalendar>()
                .ForMember(dest => dest.ExcludedDates, o => o.MapFrom(src => src.ExcludedDates.ToList()));
            CreateMap<CronCalendar, BAFCronCalendar>()
                .ForMember(dest => dest.CronExpression, o => o.MapFrom(src => src.CronExpression.CronExpressionString));
        }

        private IList<DateTimeOffset> GetExcludedDays(IEnumerable<DateTime> userDateTimes)
        {
            return userDateTimes.Select(GetDateTimeOffset).ToList();
        }

        private IList<DateTime> GetExcludedDays(IEnumerable<DateTimeOffset> userDateTimes)
        {
            return userDateTimes.Select(GetDateTime).ToList();
        }

        private bool[] GetExcludedDaysOfMonth(IEnumerable<int> excludedDays)
        {
            var result = new List<bool>();
            var hashSet = new HashSet<int>(excludedDays);
            for (var i = 0; i < 31; i++)
            {
                result.Add(hashSet.Contains((i+1)));
            }
            return result.ToArray();
        }

        private static IList<int> GetExcludedDaysOfMonth(IEnumerable<bool> excludedDayOfMonth)
        {
            return excludedDayOfMonth.Where(o => o).Select((item, idx) => (idx+1)).ToList();
        }

        private static IList<DayOfWeek> GetExcludedDaysOfWeek(IEnumerable<bool> excludedDayOfWeeks)
        {
            return excludedDayOfWeeks.Where(o => o).Select((item, idx) => (DayOfWeek) idx).ToList();
        }

        private static bool[] GetExcludedDaysOfweek(IEnumerable<DayOfWeek> excludedDayOfWeeks)
        {
            var result = new List<bool>();
            var hashSet = new HashSet<DayOfWeek>(excludedDayOfWeeks);
            for (var i = 0; i < 7; i++)
            {
                result.Add(hashSet.Contains((DayOfWeek)i));
            }
            return result.ToArray();
        }

        private DateTimeOffset CurrentUserDateTimeOffset()
        {
            return _userTask.Value.GetCurrentUser().GetCurrentDateTimeOffset();
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