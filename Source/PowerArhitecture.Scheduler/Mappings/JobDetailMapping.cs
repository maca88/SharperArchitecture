using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAF.Common.Scheduler;
using BAF.Scheduler.Extensions;
using Quartz;
using Quartz.Impl;

namespace BAF.Scheduler.Mappings
{
    public class JobDetailMapping : Profile
    {
        protected override void Configure()
        {
            CreateMap<JobDetail, IJobDetail>()
                .ForMember(dest => dest.JobDataMap, o => o.MapFrom(src => new JobDataMap((IDictionary<string, object>)src.JobData.ToDictionary(d => d.Name, d => d.Value))))
                .ForMember(dest => dest.Key, o => o.MapFrom(src => new JobKey(src.Name, src.Group)));
                
            
            CreateMap<IJobDetail, JobDetail>()
                .ForMember(dest => dest.Name, o => o.MapFrom(src => src.Key.Name))
                .ForMember(dest => dest.Group, o => o.MapFrom(src => src.Key.Group))
                .ForMember(dest => dest.JobData, o => o.MapFrom(src => src.JobDataMap.Select(p => new Parameter{Name = p.Key, Value = p.Value}).ToList()))
                .ForMember(dest => dest.InterruptableJob, o => o.MapFrom(src => typeof(IInterruptableJob).IsAssignableFrom(src.JobType)))
                .ForMember(dest => dest.CreatedBy, o => o.MapFrom(src => src.GetCreatedBy()))
                .ForMember(dest => dest.DateCreated, o => o.MapFrom(src => src.GetDateCreated()))
                .ForMember(dest => dest.Triggers, o => o.Ignore())
                .ForMember(dest => dest.RunningSince, o => o.Ignore());
        }
    }
}
