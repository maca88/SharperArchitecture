using System.Collections.Generic;
using System.Threading;
using NHibernate;
using Ninject.Syntax;
using PowerArhitecture.DataAccess.Managers;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ISessionManager
    {
        IEnumerable<ISession> GetAll();

        IEnumerable<ISession> GetAll(Thread thread);

        SessionManager.SessionInfo GetSessionInfo(ISession session);

    }
}