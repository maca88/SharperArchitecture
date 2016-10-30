using System;
using System.Threading.Tasks;
using PowerArhitecture.DataAccess.Enums;
using NHibernate;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface ISessionSubscriptionManager
    {
        void Subscribe(SessionSubscription kind, ISession session, Action action);

        void Subscribe(SessionSubscription kind, ISession session, Action<ISession> action);
    }
}