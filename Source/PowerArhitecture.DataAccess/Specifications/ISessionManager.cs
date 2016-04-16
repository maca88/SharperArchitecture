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

        SessionManager.SessionInfo Add(ISession session);

        bool Remove(ISession session);

        SessionManager.SessionInfo GetSessionInfo(ISession session);

    }
}