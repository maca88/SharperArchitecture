﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Events
{
    public class SessionFlushingEvent : IEvent
    {
        public SessionFlushingEvent(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }
    }
}
