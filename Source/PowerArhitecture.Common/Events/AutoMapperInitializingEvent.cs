using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace BAF.Common.Events
{
    public class AutoMapperInitializingEvent : BaseEvent<IConfiguration>
    {
        public AutoMapperInitializingEvent(IConfiguration message) : base(message)
        {
        }
    }
}
