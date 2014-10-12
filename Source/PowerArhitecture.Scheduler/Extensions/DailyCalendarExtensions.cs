using System;
using System.Reflection;
using Quartz.Impl.Calendar;

namespace BAF.Scheduler.Extensions
{
    public static class DailyCalendarExtensions
    {
        private static readonly FieldInfo RangeStartingHourOfDayFieldInfo;
        private static readonly FieldInfo RangeStartingMinuteFieldInfo;
        private static readonly FieldInfo RangeStartingSecondFieldInfo;
        private static readonly FieldInfo RangeStartingMillisFieldInfo;
        private static readonly FieldInfo RangeEndingHourOfDayFieldInfo;
        private static readonly FieldInfo RangeEndingMinuteFieldInfo;
        private static readonly FieldInfo RangeEndingSecondFieldInfo;
        private static readonly FieldInfo RangeEndingMillisFieldInfo;
        private static readonly FieldInfo InvertTimeRangeFieldInfo;

        static DailyCalendarExtensions()
        {
            var type = typeof (DailyCalendar);
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            RangeStartingHourOfDayFieldInfo = type.GetField("rangeStartingHourOfDay", flags);
            RangeStartingMinuteFieldInfo = type.GetField("rangeStartingMinute", flags);
            RangeStartingSecondFieldInfo = type.GetField("rangeStartingSecond", flags);
            RangeStartingMillisFieldInfo = type.GetField("rangeStartingMillis", flags);

            RangeEndingHourOfDayFieldInfo = type.GetField("rangeEndingHourOfDay", flags);
            RangeEndingMinuteFieldInfo = type.GetField("rangeEndingMinute", flags);
            RangeEndingSecondFieldInfo = type.GetField("rangeEndingSecond", flags);
            RangeEndingMillisFieldInfo = type.GetField("rangeEndingMillis", flags);

            InvertTimeRangeFieldInfo = type.GetField("invertTimeRange", flags);
        }

        public static TimeSpan GetExcludedStartTime(this DailyCalendar calendar)
        {
            var hour = (int)RangeStartingHourOfDayFieldInfo.GetValue(calendar);
            var min = (int)RangeStartingMinuteFieldInfo.GetValue(calendar);
            var sec = (int)RangeStartingSecondFieldInfo.GetValue(calendar);
            var milsec = (int)RangeStartingMillisFieldInfo.GetValue(calendar);
            return new TimeSpan(0, hour, min, sec, milsec);
        }

        public static TimeSpan GetExcludedEndTime(this DailyCalendar calendar)
        {
            var hour = (int)RangeEndingHourOfDayFieldInfo.GetValue(calendar);
            var min = (int)RangeEndingMinuteFieldInfo.GetValue(calendar);
            var sec = (int)RangeEndingSecondFieldInfo.GetValue(calendar);
            var milsec = (int)RangeEndingMillisFieldInfo.GetValue(calendar);
            return new TimeSpan(0, hour, min, sec, milsec);
        }

        public static bool GetInvertTimeRange(this DailyCalendar calendar)
        {
            return (bool)InvertTimeRangeFieldInfo.GetValue(calendar);
        }

    }
}
