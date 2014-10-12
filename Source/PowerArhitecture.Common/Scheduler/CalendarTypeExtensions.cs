using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public static class CalendarTypeExtensions
    {
        public static BaseCalendar CreateNewCalendar(this CalendarType type)
        {
            switch (type)
            {
                case CalendarType.Daily:
                    return new DailyCalendar();
                case CalendarType.Weekly:
                    return new WeeklyCalendar();
                case CalendarType.Monthly:
                    return new MonthlyCalendar();
                case CalendarType.Annual:
                    return new AnnualCalendar();
                case CalendarType.Holiday:
                    return new HolidayCalendar();
                case CalendarType.Cron:
                    return new CronCalendar();
                default:
                    throw new NotSupportedException(string.Format("not supported calendar type '{0}'", type));
            }
        }

        public static Type GetInstanceType(this CalendarType type)
        {
            switch (type)
            {
                case CalendarType.Daily:
                    return typeof(DailyCalendar);
                case CalendarType.Weekly:
                    return typeof(WeeklyCalendar);
                case CalendarType.Monthly:
                    return typeof(MonthlyCalendar);
                case CalendarType.Annual:
                    return typeof(AnnualCalendar);
                case CalendarType.Holiday:
                    return typeof(HolidayCalendar);
                case CalendarType.Cron:
                    return typeof(CronCalendar);
                default:
                    throw new NotSupportedException(string.Format("not supported calendar type '{0}'", type));
            }
        }
    }
}
