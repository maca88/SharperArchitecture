﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    public class EntityDeletingAsyncEvent : IAsyncEvent
    {
        public EntityDeletingAsyncEvent(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }
    }
}