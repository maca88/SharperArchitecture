using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace BAF.Scheduler.Extensions
{
    public static class JobDetailExtensions
    {
        public static string GetCreatedBy(this IJobDetail jobDetail)
        {
            var bafJob = jobDetail as BAFJobDetailImpl;
            return bafJob != null ? bafJob.CreatedBy : null;
        }

        public static DateTimeOffset? GetDateCreated(this IJobDetail jobDetail)
        {
            var bafJob = jobDetail as BAFJobDetailImpl;
            return bafJob != null ? (DateTimeOffset?)bafJob.DateCreated : null;
        }
    }
}
