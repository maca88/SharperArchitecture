using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Event;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.Domain;
using Ninject.Extensions.Logging;

namespace PowerArhitecture.DataAccess.EventListeners
{
    //End trimming Id for Entity<string> types: http://support.microsoft.com/kb/316626
    public class EntityPreUpdateInsertEventListener : 
        IPreInsertEventListener,
        IPreUpdateEventListener
    {
        private readonly ILogger _logger;

        public EntityPreUpdateInsertEventListener(ILogger logger)
        {
            _logger = logger;
        }

        private void EndTrim(Entity<string> entity)
        {
            if (entity == null || !entity.Id.EndsWith(" ")) return;
            _logger.Warn("End trimming empty spaces from entity '{0}' with Id '{1}'", entity.GetTypeUnproxied(), entity.Id);
            entity.SetMemberValue(o => o.Id, entity.Id.TrimEnd(' '));
        }

        public Task<bool> OnPreInsertAsync(PreInsertEvent @event)
        {
            return Task.FromResult(OnPreInsert(@event));
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            EndTrim(@event.Entity as Entity<string>);
            return false;
        }

        public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event)
        {
            return Task.FromResult(OnPreUpdate(@event));
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            EndTrim(@event.Entity as Entity<string>);
            return false;
        }
    }
}
