using System;
using PowerArhitecture.DataAccess.Enums;
using NHibernate;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ISessionEventListener
    {
        void AddAListener(SessionListenerType type, ISession session, Action action);

        void AddAListener(SessionListenerType type, ISession session, Action<ISession> action);
    }
}