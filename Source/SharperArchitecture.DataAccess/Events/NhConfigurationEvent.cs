using SharperArchitecture.Common.Events;
using NHibernate.Cfg;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
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
