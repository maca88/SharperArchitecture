using System;
using Quartz.Spi;

namespace BAF.Scheduler.Specifications
{
    public interface IAuditTrigger : IOperableTrigger
    {
        string CreatedBy { get; set; }

        DateTimeOffset DateCreated { get; set; }
    }
}