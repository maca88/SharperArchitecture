using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.DataAccess.Events
{
    public class SessionFlushingAsyncEvent : IAsyncEvent
    {
        public SessionFlushingAsyncEvent(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }
    }
}
