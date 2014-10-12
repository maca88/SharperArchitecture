namespace BAF.Common.Scheduler
{
    public enum SchedulerLogType
    {
        JobVetoed = 1,
        JobToBeExecuted,
        JobWasExecuted,
        JobScheduled,
        JobUnscheduled,
        JobAdded,
        JobDeleted,
        JobPaused,
        JobsPaused,
        JobResumed,
        JobsResumed,

        TriggerFired,
        TriggerMisfired,
        TriggerComplete,
        TriggerFinalized,
        TriggerPaused,
        TriggersPaused,
        TriggerResumed,
        TriggersResumed,

        SchedulerError,
        SchedulerInStandbyMode,
        SchedulerStarting,
        SchedulerStarted,
        SchedulerShutdown,
        SchedulerShuttingdown,
        SchedulingDataCleared
    }
}
