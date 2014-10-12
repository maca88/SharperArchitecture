using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NHibernate
{
    public static class QueryOverExtensions
    {
        public static IQueryOver<TRoot, TSubType> Lock<TRoot, TSubType>(this IQueryOver<TRoot, TSubType> queryOver, PowerArhitecture.DataAccess.Enums.LockMode lockMode)
        {
            switch (lockMode)
            {
                case PowerArhitecture.DataAccess.Enums.LockMode.None:
                    return queryOver.Lock().None;
                case PowerArhitecture.DataAccess.Enums.LockMode.Read:
                    return queryOver.Lock().Read;
                case PowerArhitecture.DataAccess.Enums.LockMode.Upgrade:
                    return queryOver.Lock().Upgrade;
                case PowerArhitecture.DataAccess.Enums.LockMode.UpgradeNoWait:
                    return queryOver.Lock().UpgradeNoWait;
                case PowerArhitecture.DataAccess.Enums.LockMode.Write:
                    return queryOver.Lock().Write;
                case PowerArhitecture.DataAccess.Enums.LockMode.Force:
                    return queryOver.Lock().Force;
            }
            return queryOver;
        }

        public static IQueryOver<TRoot, TSubType> Where<TRoot, TSubType>(this IQueryOver<TRoot, TSubType> queryOver, IEnumerable<Expression<Func<TSubType, bool>>> expressions)
        {
            return expressions.Aggregate(queryOver, (current, expression) => current.Where(expression));
        }

        public static IQueryOver<TRoot, TSubType> Fetch<TRoot, TSubType>(this IQueryOver<TRoot, TSubType> queryOver,
                                                                         string associationPath, FetchMode fetchMode)
        {
            queryOver.UnderlyingCriteria.SetFetchMode(associationPath, fetchMode);
            return queryOver;
        }
    }
}
