using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Intercept;
using NHibernate.Persister.Entity;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Proxy;
using NHibernate.Type;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Decorators;
using SharperArchitecture.DataAccess.Providers;

namespace NHibernate
{
    public static class SessionExtensions
    {
        private static readonly PropertyInfo IsAlreadyDisposedPropInfo;

        static SessionExtensions()
        {
            IsAlreadyDisposedPropInfo = typeof (SessionImpl).GetProperty("IsAlreadyDisposed", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static bool IsAlreadyDisposed(this ISession session)
        {
            var sessionImpl = session.GetSessionImplementation();
            return (bool)IsAlreadyDisposedPropInfo.GetValue(sessionImpl);
        }

        public static long GetCreatedTimestamp(this ISession session)
        {
            var sessionImpl = (SessionImpl)session.GetSessionImplementation();
            return sessionImpl?.Timestamp ?? 0;
        }

        public static void RollbackTransaction(this ISession session)
        {
            if (session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return;
            session.Transaction.Rollback();
            session.Transaction.Dispose();
        }

        public static void RollbackTransaction(this IStatelessSession session)
        {
            if (session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return;
            session.Transaction.Rollback();
            session.Transaction.Dispose();
        }

        public static bool CommitTransaction(this ISession session)
        {
            if (session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return false;
            session.Transaction.Commit();
            return true;
        }
        public static async Task<bool> CommitTransactionAsync(this ISession session)
        {
            if (session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return false;
            await session.Transaction.CommitAsync();
            return true;
        }

        public static bool CommitTransaction(this IStatelessSession session)
        {
            if (session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return false;
            session.Transaction.Commit();
            return true;
        }

        internal static ISession Unwrap(this ISession session)
        {
            var wrap = session as SessionDecorator;
            return wrap != null ? wrap.Session : session;
        }

        internal static bool IsManaged(this ISession session)
        {
            return SessionProvider.RegisteredSessionIds.Contains(session.GetSessionImplementation().SessionId);
        }
    }
}
