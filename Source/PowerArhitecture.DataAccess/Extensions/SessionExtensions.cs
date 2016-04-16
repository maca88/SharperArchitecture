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
            return sessionImpl == null ? 0 : sessionImpl.Timestamp;
        }

        public static void RollbackTransaction(this ISession session)
        {
            if (session.Transaction == null || session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return;
            session.Transaction.Rollback();
            session.Transaction.Dispose();
        }

        public static void RollbackTransaction(this IStatelessSession session)
        {
            if (session.Transaction == null || session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return;
            session.Transaction.Rollback();
            session.Transaction.Dispose();
        }

        public static bool CommitTransaction(this ISession session)
        {
            if (session.Transaction == null || session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return false;
            try
            {
                session.Transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                RollbackTransaction(session);
                throw;
            }
        }
        public static async Task<bool> CommitTransactionAsync(this ISession session)
        {
            if (session.Transaction == null || session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return false;
            try
            {
                await session.Transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                RollbackTransaction(session);
                throw;
            }
        }

        public static bool CommitTransaction(this IStatelessSession session)
        {
            if (session.Transaction == null || session.Transaction.WasRolledBack || !session.Transaction.IsActive)
                return false;
            try
            {
                session.Transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                RollbackTransaction(session);
                throw;
            }
        }
    }
}
