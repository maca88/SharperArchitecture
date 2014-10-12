using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.Scheduler.Specifications;
using Quartz;

namespace BAF.Scheduler.Extensions
{
    public static class TriggerExtensions
    {
        public static string GetCreatedBy(this ITrigger trigger)
        {
            var auditTrigger = trigger as IAuditTrigger;
            return auditTrigger != null ? auditTrigger.CreatedBy : null;
        }

        public static DateTimeOffset? GetDateCreated(this ITrigger trigger)
        {
            var auditTrigger = trigger as IAuditTrigger;
            return auditTrigger != null ? (DateTimeOffset?)auditTrigger.DateCreated : null;
        }
    }
}
