using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAF.Common.Notifications;
using BAF.Notifications.Entities;

namespace BAF.Notifications.Mappings
{
    public class NotificationMapping : Profile
    {
        protected override void Configure()
        {
            CreateMap<Notification, NotificationMessage>()
                .ForMember(dest => dest.Recipients, o => o.MapFrom(src => src.Recipients.Select(r => r.Recipient.UserName)));
        }
    }
}
