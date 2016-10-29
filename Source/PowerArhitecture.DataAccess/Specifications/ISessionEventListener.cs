using System;
using System.Threading.Tasks;
using PowerArhitecture.DataAccess.Enums;
using NHibernate;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ISessionEventProvider
    {
        void AddListener(SessionListenerType type, ISession session, Action action);

        void AddListener(SessionListenerType type, ISession session, Action<ISession> action);
    }
}