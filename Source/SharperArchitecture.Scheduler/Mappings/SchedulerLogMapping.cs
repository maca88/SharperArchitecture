using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAF.Common.Scheduler;
using BAF.Scheduler.Entities;
using Quartz;

namespace BAF.Scheduler.Mappings
{
    public class SchedulerLogMapping : Profile
    {
        protected override void Configure()
        {
            CreateMap<TriggerKey, SchedulerLog>()
                .ForMember(dest => dest.Id, o => o.Ignore())
                .ForMember(dest => dest.SchedulerName, o => o.Ignore())
                .ForMember(dest => dest.Error, o => o.Ignore())
                .ForMember(dest => dest.Date, o => o.Ignore())
                .ForMember(dest => dest.Type, o => o.Ignore())
                .ForMember(dest => dest.JobGroup, o => o.Ignore())
                .ForMember(dest => dest.JobName, o => o.Ignore())
                .ForMember(dest => dest.TriggerName, o => o.MapFrom(src => src.Name))
                .ForMember(dest => dest.TriggerGroup, o => o.MapFrom(src => src.Group));

            CreateMap<ITrigger, SchedulerLog>()
                .ForMember(dest => dest.Id, o => o.Ignore())
                .ForMember(dest => dest.SchedulerName, o => o.Ignore())
                .ForMember(dest => dest.Error, o => o.Ignore())
                .ForMember(dest => dest.Date, o => o.Ignore())
                .ForMember(dest => dest.Type, o => o.Ignore())
                .ForMember(dest => dest.JobGroup, o => o.MapFrom(src => src.JobKey.Group))
                .ForMember(dest => dest.JobName, o => o.MapFrom(src => src.JobKey.Name))
                .ForMember(dest => dest.TriggerName, o => o.MapFrom(src => src.Key.Name))
                .ForMember(dest => dest.TriggerGroup, o => o.MapFrom(src => src.Key.Group));

            CreateMap<IJobExecutionContext, SchedulerLog>()
                .ForMember(dest => dest.Id, o => o.Ignore())
                .ForMember(dest => dest.SchedulerName, o => o.Ignore())
                .ForMember(dest => dest.Error, o => o.Ignore())
                .ForMember(dest => dest.Date, o => o.Ignore())
                .ForMember(dest => dest.Type, o => o.Ignore())
                .ForMember(dest => dest.JobGroup, o => o.MapFrom(src => src.JobDetail.Key.Group))
                .ForMember(dest => dest.JobName, o => o.MapFrom(src => src.JobDetail.Key.Name))
                .ForMember(dest => dest.TriggerName, o => o.MapFrom(src => src.Trigger.Key.Name))
                .ForMember(dest => dest.TriggerGroup, o => o.MapFrom(src => src.Trigger.Key.Group));


            CreateMap<IJobDetail, SchedulerLog>()
                .ForMember(dest => dest.Id, o => o.Ignore())
                .ForMember(dest => dest.SchedulerName, o => o.Ignore())
                .ForMember(dest => dest.Error, o => o.Ignore())
                .ForMember(dest => dest.Date, o => o.Ignore())
                .ForMember(dest => dest.Type, o => o.Ignore())
                .ForMember(dest => dest.JobGroup, o => o.MapFrom(src => src.Key.Group))
                .ForMember(dest => dest.JobName, o => o.MapFrom(src => src.Key.Name))
                .ForMember(dest => dest.TriggerName, o => o.Ignore())
                .ForMember(dest => dest.TriggerGroup, o => o.Ignore());

            CreateMap<JobKey, SchedulerLog>()
                .ForMember(dest => dest.Id, o => o.Ignore())
                .ForMember(dest => dest.SchedulerName, o => o.Ignore())
                .ForMember(dest => dest.Error, o => o.Ignore())
                .ForMember(dest => dest.Date, o => o.Ignore())
                .ForMember(dest => dest.Type, o => o.Ignore())
                .ForMember(dest => dest.JobGroup, o => o.MapFrom(src => src.Group))
                .ForMember(dest => dest.JobName, o => o.MapFrom(src => src.Name))
                .ForMember(dest => dest.TriggerName, o => o.Ignore())
                .ForMember(dest => dest.TriggerGroup, o => o.Ignore());
        }
    }
}
