using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAF.Common.Scheduler;
using Quartz;

namespace BAF.Scheduler.Mappings
{
    public class SchedulerInfoMapping : Profile
    {
        protected override void Configure()
        {
            CreateMap<SchedulerMetaData, SchedulerInfo>()
                .ForMember(dest => dest.Summary, o => o.MapFrom(src => src.GetSummary()));
        }
    }
}
