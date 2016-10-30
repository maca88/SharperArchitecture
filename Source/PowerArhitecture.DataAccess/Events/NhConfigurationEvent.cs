using PowerArhitecture.Common.Events;
using NHibernate.Cfg;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    public class NhConfigurationEvent : IEvent
    {
        public NhConfigurationEvent(Configuration configuration)
        {
            Configuration = configuration;
        }

        public Configuration Configuration { get; }
    }
}
