﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Event;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess.Events;
using SharperArchitecture.DataAccess.Extensions;
using SharperArchitecture.Domain;

namespace SharperArchitecture.DataAccess.EventListeners
{
    //End trimming Id for Entity<string> types: http://support.microsoft.com/kb/316626
    internal class EntityPreUpdateInsertEventListener : 
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

        public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
        {
            return Task.FromResult(OnPreInsert(@event));
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            EndTrim(@event.Entity as Entity<string>);
            return false;
        }

        public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
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
