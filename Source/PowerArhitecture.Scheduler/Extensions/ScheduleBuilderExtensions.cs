using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using BAF.Common.Scheduler;
using Quartz;

namespace BAF.Scheduler.Extensions
{
    public static class ScheduleBuilderExtensions
    {
        public static DailyTimeIntervalScheduleBuilder WithMisfireHandlingInstruction(
            this DailyTimeIntervalScheduleBuilder builder, int instruction)
        {
            var instructionEnum = (TriggerMisfireInstruction) instruction;
            switch (instructionEnum)
            {
                case TriggerMisfireInstruction.DoNothing:
                    builder.WithMisfireHandlingInstructionDoNothing();
                    break;
                case TriggerMisfireInstruction.FireOnceNow:
                    builder.WithMisfireHandlingInstructionFireAndProceed();
                    break;
                case TriggerMisfireInstruction.IgnoreMisfirePolicy:
                    builder.WithMisfireHandlingInstructionIgnoreMisfires();
                    break;
                default:
                    return builder;
            }
            return builder;
        }

        public static CronScheduleBuilder WithMisfireHandlingInstruction(
            this CronScheduleBuilder builder, int instruction)
        {
            var instructionEnum = (TriggerMisfireInstruction)instruction;
            switch (instructionEnum)
            {
                case TriggerMisfireInstruction.DoNothing:
                    builder.WithMisfireHandlingInstructionDoNothing();
                    break;
                case TriggerMisfireInstruction.FireOnceNow:
                    builder.WithMisfireHandlingInstructionFireAndProceed();
                    break;
                case TriggerMisfireInstruction.IgnoreMisfirePolicy:
                    builder.WithMisfireHandlingInstructionIgnoreMisfires();
                    break;
                default:
                    return builder;
            }
            return builder;
        }
    }
}
