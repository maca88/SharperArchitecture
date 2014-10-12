namespace BAF.Common.Scheduler
{
    public enum TriggerMisfireInstruction
    {
        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that the
        /// <see cref="ITrigger" /> will never be evaluated for a misfire situation,
        /// and that the scheduler will simply try to fire it as soon as it can,
        /// and then update the Trigger as if it had fired at the proper time.
        /// </summary>
        /// <remarks>
        /// NOTE: if a trigger uses this instruction, and it has missed 
        /// several of its scheduled firings, then several rapid firings may occur 
        /// as the trigger attempt to catch back up to where it would have been. 
        /// For example, a SimpleTrigger that fires every 15 seconds which has 
        /// misfired for 5 minutes will fire 20 times once it gets the chance to 
        /// fire.
        /// </remarks>
        IgnoreMisfirePolicy = -1,

        /// <summary>
        /// Use smart policy.
        /// </summary>
        SmartPolicy = 0,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that upon a mis-fire
        /// situation, the <see cref="IDailyTimeIntervalTrigger" /> wants to be
        /// fired now by <see cref="IScheduler" />.
        /// </summary>
        FireOnceNow = 1,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that upon a mis-fire
        /// situation, the <see cref="DailyTimeIntervalTrigger" /> wants to have it's
        /// next-fire-time updated to the next time in the schedule after the
        /// current time (taking into account any associated <see cref="ICalendar" />,
        /// but it does not want to be fired now.
        /// </summary>
        DoNothing = 2
    }
}