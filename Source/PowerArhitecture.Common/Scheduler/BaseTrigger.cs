using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public abstract class BaseTrigger
    {
        protected BaseTrigger()
        {
            JobData = new List<Parameter>();
        }

        public abstract TriggerType Type { get; }

        public string Name { get; set; }

        public string Group { get; set; }

        public string JobName { get; set; }

        public string JobGroup { get; set; }

        public string Description { get; set; }

        public TriggerStatus Status { get; set; }

        public DateTime? NextFireTimeUtc { get; set; }

        public DateTime? PreviousFireTimeUtc { get; set; }

        public string CalendarName { get; set; }

        public IList<Parameter> JobData { get; set; }

        public DateTime? FinalFireTimeUtc { get; set; }

        public DateTime? EndTimeUtc { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public int Priority { get; set; }

        public string CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public int MisfireInstruction { get; set; }

        public IDictionary<string, object> GetJobData()
        {
            return JobData.ToDictionary(o => o.Name, o => o.Value);
        }

        public override bool Equals(object obj)
        {
            var trigger = obj as BaseTrigger;
            if (trigger != null && trigger.Type == Type)
            {
                return Equals(trigger.Name, Name) && Equals(trigger.Group, Group);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Name))
                return base.GetHashCode();
            unchecked
            {
                var result = 37; // prime
                result *= 397; // also prime (see note)
                if (Name != null)
                    result += Name.GetHashCode();

                result *= 397;
                if (Group != null)
                    result += Group.GetHashCode();
                return result;
            }
        }
    }
}
