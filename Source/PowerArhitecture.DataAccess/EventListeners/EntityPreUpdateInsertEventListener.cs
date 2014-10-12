using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.Domain;
using Ninject.Extensions.Logging;

namespace PowerArhitecture.DataAccess.EventListeners
{
    //End trimming Id for Entity<string> types: http://support.microsoft.com/kb/316626
    public class EntityPreUpdateInsertEventListener : 
        IListener<EntityPreInsertingEvent>,
        IListener<EntityPreUpdatingEvent>
    {
        private readonly ILogger _logger;

        public EntityPreUpdateInsertEventListener(ILogger logger)
        {
            _logger = logger;
        }

        public void Handle(EntityPreInsertingEvent message)
        {
            EndTrim(message.Message.Entity as Entity<string>);
        }

        public void Handle(EntityPreUpdatingEvent message)
        {
            EndTrim(message.Message.Entity as Entity<string>);
        }

        private void EndTrim(Entity<string> entity)
        {
            if (entity == null || !entity.Id.EndsWith(" ")) return;
            _logger.Warn("End trimming empty spaces from entity '{0}' with Id '{1}'", entity.GetTypeUnproxied(), entity.Id);
            entity.SetMemberValue(o => o.Id, entity.Id.TrimEnd(' '));
        }
    }
}
