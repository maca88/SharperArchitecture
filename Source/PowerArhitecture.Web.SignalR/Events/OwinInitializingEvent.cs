using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using Owin;

namespace PowerArhitecture.Web.SignalR.Events
{
    public class OwinInitializingEvent : BaseEvent<IAppBuilder>
    {
        public OwinInitializingEvent(IAppBuilder message) : base(message)
        {
        }
    }
}
