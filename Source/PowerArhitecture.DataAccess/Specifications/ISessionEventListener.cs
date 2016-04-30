using System;
using System.Threading.Tasks;
using PowerArhitecture.DataAccess.Enums;
using NHibernate;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ISessionEventProvider
    {
        void AddAListener(SessionListenerType type, ISession session, Func<Task> action);

        void AddAListener(SessionListenerType type, ISession session, Func<ISession, Task> action);
    }
}