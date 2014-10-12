using System.Collections.Generic;
using System.Threading;
using NHibernate;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ISessionManager
    {
        IEnumerable<ISession> GetAll();

        IEnumerable<ISession> GetAll(Thread thread);

        SessionProperties GetSessionProperties(ISession session);

    }
}