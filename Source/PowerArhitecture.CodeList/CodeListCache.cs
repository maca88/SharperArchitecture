using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PowerArhitecture.CodeList.Specifications;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.DataCaching;
using PowerArhitecture.DataCaching.Specifications;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Ninject.Extensions.Logging;

namespace PowerArhitecture.CodeList
{
    public class CodeListCache : BaseApplicationCache, ICodeListCache
    {
        private readonly Lazy<ISessionFactory> _sessionFactory; //Must be lazy so that listeners will be created before SessionFactory
        private const string CodeListPostfixKey = "CodeList";

        public CodeListCache(ILogger logger, IDataCache dataCache, Lazy<ISessionFactory> sessionFactory, IEventAggregator eventAggregator,
            ISessionEventListener sessionEventListener, IDataCachingSettings settings)
            : base(eventAggregator, logger, dataCache, sessionEventListener, settings)
        {
            _sessionFactory = sessionFactory;
        }

        public override void Initialize()
        {
            SetMonitoringTypes();
        }

        private void SetMonitoringTypes()
        {
            MonitoringTypes.Add(typeof (ICodeList));
            MonitoringTypes.Add(typeof (ICodeListLocalization));
        }

        public override void Refresh()
        {
            var codeListTypes = _sessionFactory.Value.GetAllClassMetadata()
                .Select(o => o.Value.GetMappedClass(EntityMode.Poco))
                .Where(t => typeof(ICodeList).IsAssignableFrom(t));
            UpdateOrInsertCodeLists(codeListTypes);
        }

        protected override void SessionCommited(ISession session)
        {
            ConcurrentDictionary<object, object> dict;
            ModifiedObjects.TryRemove(session, out dict);
            if(!dict.Any()) return;
            var toUpdate = new HashSet<Type>();
            foreach (var pair in dict)
            {
                var codeList = pair.Key as ICodeList;
                if (codeList != null)
                {
                    toUpdate.Add(codeList.GetType());
                }
                var codeListLoc = pair.Key as ICodeListLocalization;
                if (codeListLoc != null)
                {
                    toUpdate.Add(codeListLoc.GetCodeList().GetType());
                }
            }
            UpdateOrInsertCodeLists(toUpdate.ToList());
        }

        public void UpdateOrInsertCodeList<TType>()
        {
            UpdateOrInsertCodeLists(new List<Type> {typeof (TType)});
        }

        public void UpdateOrInsertCodeList(Type codeListType)
        {
            UpdateOrInsertCodeLists(new List<Type> {codeListType});
        }

        public IEnumerable<TType> GetCodeList<TType>() where TType : ICodeList
        {
            return DataCache.Get<IEnumerable<TType>>(GetCodeListCacheKey(typeof (TType)));
        }

        public TType GetCodeList<TType>(string code) where TType : ICodeList
        {
            return DataCache.Get<IEnumerable<TType>>(GetCodeListCacheKey(typeof(TType))).FirstOrDefault(o => o.Code == code);
        }

        public void UpdateOrInsertCodeLists(IEnumerable<Type> codeListTypes)
        {
            try
            {
                var session = _sessionFactory.Value.OpenSession();
                session.BeginTransaction();
                session.FlushMode = FlushMode.Never;
                //Expression<Func<ICodeListLoc<ICodeList, ICodeListLocalization<ICodeList, ICodeListLocalization>>, object>> namesExpr = list => list.Names;
                const string namesPath = "Names"; //ExpressionProcessor.FindMemberExpression(namesExpr.Body);
                const string languagePath = "Names.Language";
                var languageAlias = languagePath.Replace(".", "");
                var futures = new Dictionary<Type, IEnumerable>();

                //Fill the dictionary with future queries
                foreach (var codeListType in codeListTypes)
                {
                    var localizable = !typeof (ICodeListNoLoc).IsAssignableFrom(codeListType);

                    var codeListQuery = session
                        .CreateCriteria(codeListType, CodeListPostfixKey)
                        .SetLockMode(CodeListPostfixKey, LockMode.Write);
                    if (localizable)
                    {
                        codeListQuery = codeListQuery
                            .CreateAlias(namesPath, namesPath, JoinType.LeftOuterJoin)
                            .CreateAlias(languagePath, languageAlias, JoinType.LeftOuterJoin)
                            .SetLockMode(namesPath, LockMode.Write)
                            .SetLockMode(languageAlias, LockMode.Write);
                    }
                    codeListQuery.SetResultTransformer(new DistinctRootEntityResultTransformer());

                    var future = codeListQuery.GetType().GetMethod("Future").MakeGenericMethod(codeListType).Invoke(codeListQuery, null) as IEnumerable;
                        //.Future<ICodeList>();

                    futures.Add(codeListType, future);
                }

                //Execute all queries and insert or update the cache
                foreach (var pair in futures)
                {
                    //Dictionary<CodeListCode, Dictionary<LangaugeCulture, Name>>
                    var codeListKey = GetCodeListCacheKey(pair.Key);
                    var unproxiedList = session.DeepCopy(pair.Value);
                    DataCache.InsertOrUpdate(codeListKey, unproxiedList);
                }
                session.Dispose();
            }
            catch (Exception e)
            {
                Logger.Warn("An error has occurred {0}", e);
                throw;
            }
        }

        private static string GetCodeListCacheKey(Type codeListType)
        {
            return String.Format("{0}{1}", codeListType.Name, CodeListPostfixKey);
        }
    }
}
