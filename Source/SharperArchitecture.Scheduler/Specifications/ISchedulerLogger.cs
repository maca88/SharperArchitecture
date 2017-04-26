using Quartz;

namespace BAF.Scheduler.Specifications
{
    public interface ISchedulerLogger : ISchedulerListener, IJobListener, ITriggerListener, ILogRepository
    {
        string SchedulerName { get; }
    }
}