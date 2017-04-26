using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Stat;

namespace SharperArchitecture.Tests.WebApi
{
    public class DatabaseStatistics
    {
        public long EntityDeleteCount { get; set; }
        public long EntityInsertCount { get; set; }
        public long EntityLoadCount { get; set; }
        public long EntityFetchCount { get; set; }
        public long EntityUpdateCount { get; set; }
        public long QueryExecutionCount { get; set; }
        public TimeSpan QueryExecutionMaxTime { get; set; }
        public string QueryExecutionMaxTimeQueryString { get; set; }
        public long QueryCacheHitCount { get; set; }
        public long QueryCacheMissCount { get; set; }
        public long QueryCachePutCount { get; set; }
        public long FlushCount { get; set; }
        public long ConnectCount { get; set; }
        public long SecondLevelCacheHitCount { get; set; }
        public long SecondLevelCacheMissCount { get; set; }
        public long SecondLevelCachePutCount { get; set; }
        public long SessionCloseCount { get; set; }
        public long SessionOpenCount { get; set; }
        public long CollectionLoadCount { get; set; }
        public long CollectionFetchCount { get; set; }
        public long CollectionUpdateCount { get; set; }
        public long CollectionRemoveCount { get; set; }
        public long CollectionRecreateCount { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsStatisticsEnabled { get; set; }
        public string[] Queries { get; set; }
        public string[] EntityNames { get; set; }
        public string[] CollectionRoleNames { get; set; }
        public string[] SecondLevelCacheRegionNames { get; set; }
        public long SuccessfulTransactionCount { get; set; }
        public long TransactionCount { get; set; }
        public long PrepareStatementCount { get; set; }
        public long CloseStatementCount { get; set; }
        public long OptimisticFailureCount { get; set; }
        public TimeSpan OperationThreshold { get; set; }
    }
}
