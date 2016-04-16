using System;
using PowerArhitecture.DataAccess.Enums;
using NHibernate;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ISessionEventProvider
    {
        void AddAListener(SessionListenerType type, ISession session, Action action);

        void AddAListener(SessionListenerType type, ISession session, Action<ISession> action);
    }
}