using PowerArhitecture.Common.Events;
using NHibernate.Cfg;

namespace PowerArhitecture.DataAccess.Events
{
    public class NhConfigurationEvent : BaseEvent<Configuration>
    {
        public NhConfigurationEvent(Configuration message) : base(message)
        {
        }
    }
}
