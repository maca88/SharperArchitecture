using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Envers.Configuration.Fluent;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.DataAccess.Events
{
    public class EnverConfigurationEvent : BaseEvent<NHibernate.Envers.Configuration.Fluent.FluentConfiguration>
    {
        public EnverConfigurationEvent(FluentConfiguration message) : base(message)
        {
        }
    }
}
