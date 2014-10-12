namespace BAF.Common.Scheduler
{
    public enum SimpleTriggerMisfireInstruction
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
        /// situation, the <see cref="ISimpleTrigger" /> wants to be fired
        /// now by <see cref="IScheduler" />.
        /// <para>
        /// <i>NOTE:</i> This instruction should typically only be used for
        /// 'one-shot' (non-repeating) Triggers. If it is used on a trigger with a
        /// repeat count > 0 then it is equivalent to the instruction 
        /// <see cref="RescheduleNowWithRemainingRepeatCount " />.
        /// </para>
        /// </summary>		
        FireNow = 1,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that upon a mis-fire
        /// situation, the <see cref="ISimpleTrigger" /> wants to be
        /// re-scheduled to 'now' (even if the associated <see cref="ICalendar" />
        /// excludes 'now') with the repeat count left as-is.   This does obey the
        /// <see cref="ITrigger" /> end-time however, so if 'now' is after the
        /// end-time the <see cref="ITrigger" /> will not fire again.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <i>NOTE:</i> Use of this instruction causes the trigger to 'forget'
        /// the start-time and repeat-count that it was originally setup with (this
        /// is only an issue if you for some reason wanted to be able to tell what
        /// the original values were at some later time).
        /// </para>
        /// </remarks>
        RescheduleNowWithExistingRepeatCount = 2,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that upon a mis-fire
        /// situation, the <see cref="ISimpleTrigger" /> wants to be
        /// re-scheduled to 'now' (even if the associated <see cref="ICalendar" />
        /// excludes 'now') with the repeat count set to what it would be, if it had
        /// not missed any firings. This does obey the <see cref="ITrigger" /> end-time 
        /// however, so if 'now' is after the end-time the <see cref="ITrigger" /> will 
        /// not fire again.
        /// 
        /// <para>
        /// <i>NOTE:</i> Use of this instruction causes the trigger to 'forget'
        /// the start-time and repeat-count that it was originally setup with.
        /// Instead, the repeat count on the trigger will be changed to whatever
        /// the remaining repeat count is (this is only an issue if you for some
        /// reason wanted to be able to tell what the original values were at some
        /// later time).
        /// </para>
        /// 
        /// <para>
        /// <i>NOTE:</i> This instruction could cause the <see cref="ITrigger" />
        /// to go to the 'COMPLETE' state after firing 'now', if all the
        /// repeat-fire-times where missed.
        /// </para>
        /// </summary>
        RescheduleNowWithRemainingRepeatCount = 3,

        /// <summary> 
        /// Instructs the <see cref="IScheduler" /> that upon a mis-fire
        /// situation, the <see cref="ISimpleTrigger" /> wants to be
        /// re-scheduled to the next scheduled time after 'now' - taking into
        /// account any associated <see cref="ICalendar" />, and with the
        /// repeat count set to what it would be, if it had not missed any firings.
        /// </summary>
        /// <remarks>
        /// <i>NOTE/WARNING:</i> This instruction could cause the <see cref="ITrigger" />
        /// to go directly to the 'COMPLETE' state if all fire-times where missed.
        /// </remarks>
        RescheduleNextWithRemainingCount = 4,

        /// <summary>
        /// Instructs the <see cref="IScheduler" /> that upon a mis-fire
        /// situation, the <see cref="ISimpleTrigger" /> wants to be
        /// re-scheduled to the next scheduled time after 'now' - taking into
        /// account any associated <see cref="ICalendar" />, and with the
        /// repeat count left unchanged.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <i>NOTE/WARNING:</i> This instruction could cause the <see cref="ITrigger" />
        /// to go directly to the 'COMPLETE' state if all the end-time of the trigger 
        /// has arrived.
        /// </para>
        /// </remarks>
        RescheduleNextWithExistingCount = 5
    }
}