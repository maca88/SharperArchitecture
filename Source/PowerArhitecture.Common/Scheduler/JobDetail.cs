using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class JobDetail
    {
        public JobDetail()
        {
            Triggers = new List<BaseTrigger>();
            JobData = new List<Parameter>();
        }

        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Group { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public Type JobType { get; set; }

        public IList<Parameter> JobData { get; set; }

        public bool Durable { get; set; }

        public bool RequestsRecovery { get; set; }



        public bool InterruptableJob { get; set; }

        /// <summary>
        /// Whether the associated Job class carries the <see cref="T:Quartz.DisallowConcurrentExecutionAttribute"/> attribute.
        /// 
        /// </summary>
        public bool ConcurrentExecutionDisallowed { get; set; }

        /// <summary>
        /// Whether the associated Job class carries the <see cref="P:Quartz.Impl.JobDetailImpl.PersistJobDataAfterExecution"/> attribute.
        /// 
        /// </summary>
        public bool PersistJobDataAfterExecution { get; set; }

        public DateTimeOffset? RunningSince { get; set; }

        public IList<BaseTrigger> Triggers { get; set; }  

        public ActivityStatus ActivityStatus
        {
            get
            {
                if (Triggers == null || !Triggers.Any())
                {
                    return ActivityStatus.Complete;
                }
                if (Triggers.All(a => a.Status == TriggerStatus.Complete))
                {
                    return ActivityStatus.Complete;
                }
                if (Triggers.All(a => a.Status != TriggerStatus.Paused || a.Status == TriggerStatus.Complete))
                {
                    return ActivityStatus.Active;
                }
                if (Triggers.All(a => a.Status == TriggerStatus.Paused || a.Status == TriggerStatus.Complete))
                {
                    return ActivityStatus.Paused;
                }
                return ActivityStatus.Mixed;
            }
        }

        public string CreatedBy { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
