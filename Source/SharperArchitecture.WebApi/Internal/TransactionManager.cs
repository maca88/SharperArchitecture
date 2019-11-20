using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NHibernate;
using SharperArchitecture.Common.Attributes;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Attributes;
using SharperArchitecture.DataAccess.Events;
using SharperArchitecture.DataAccess.Extensions;
using SharperArchitecture.WebApi.Specifications;
using SimpleInjector;
using System.Transactions;
using System.Web.Http.ExceptionHandling;
using NHibernate.Util;
using SharperArchitecture.Common.Events;
using Container = SimpleInjector.Container;

namespace SharperArchitecture.WebApi.Internal
{
    public interface ITransactionAttributeProvider
    {
        TransactionAttribute Get(HttpRequestMessage requestMessage, HttpActionDescriptor actionDescriptor);
    }

    public class TransactionAttributeProvider : ITransactionAttributeProvider
    {
        private readonly TransactionAttribute _defaultTransaction = new TransactionAttribute();

        public virtual TransactionAttribute Get(HttpRequestMessage requestMessage, HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor?.GetCustomAttributes<TransactionAttribute>().SingleOrDefault() ?? _defaultTransaction;
        }
    }

    /// <summary>
    /// Responsable for managing a transaction inside a WebApi request.
    /// </summary>
    [Priority(short.MaxValue)]
    public class TransactionManager : DelegatingHandler, IEventHandler<SessionCreatedEvent>
    {
        private readonly IRequestMessageProvider _requestMessageProvider;
        private readonly ITransactionAttributeProvider _transactionAttributeProvider;
        private readonly Container _container;
        private readonly ILogger _logger;
        private const string ScopeKey = "WebApiReqeustInfo";
        private static readonly ConcurrentDictionary<string, AsyncLock> RequestLocks = new ConcurrentDictionary<string, AsyncLock>(StringComparer.OrdinalIgnoreCase);
        

        private class WebApiRequestInfo
        {
            public WebApiRequestInfo(TransactionAttribute transaction, TransactionScope transactionScope = null)
            {
                Transaction = transaction;
                TransactionScope = transactionScope;
            }

            public Dictionary<string, ISession> Sessions { get; } = new Dictionary<string, ISession>();

            public TransactionScope TransactionScope { get; }

            public TransactionAttribute Transaction { get; }
        }

        public TransactionManager(IRequestMessageProvider requestMessageProvider, ITransactionAttributeProvider transactionAttributeProvider, Container container, ILogger logger)
        {
            _requestMessageProvider = requestMessageProvider;
            _transactionAttributeProvider = transactionAttributeProvider;
            _container = container;
            _logger = logger;
        }

        public static void AddSequentialAbsolutePath(string absolutePath)
        {
            RequestLocks.TryAdd(absolutePath, new AsyncLock());
        }

        public void Handle(SessionCreatedEvent @event)
        {
            var currentMessage = _requestMessageProvider.CurrentMessage;
            var scope = Lifestyle.Scoped.GetCurrentScope(_container);
            if (currentMessage == null || scope == null || scope.IsOwnedByUnitOfWork())
            {
                return;
            }

            var actionDescriptor = currentMessage.GetActionDescriptor();
            var info = GetWebApiRequestInfo();
            if (info == null)
            {
                var transaction = _transactionAttributeProvider.Get(currentMessage, actionDescriptor);
                info = CreateRequestInfo(transaction);
                scope.SetItem(ScopeKey, info);
            }

            if (info.Sessions.ContainsKey(@event.DatabaseConfigurationName))
            {
                throw new NotSupportedException("Having multiple sessions for one database configuration within the same WebApi request is not supported. " +
                                                $"Multiple sessions detected for database configuration with name: {@event.DatabaseConfigurationName}. " +
                                                "Hint: method container.BeginExecutionContextScope must not be called inside a WebApi request");
            }
            
            SetupSession(@event.Session, info.Transaction, currentMessage, actionDescriptor);
            info.Sessions.Add(@event.DatabaseConfigurationName, @event.Session);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Here GetActionDescriptor is not yet available
            if (!RequestLocks.ContainsKey(request.RequestUri.AbsolutePath))
            {
                return await InternalSendAsync();
            }
            using (await RequestLocks.GetOrAdd(request.RequestUri.AbsolutePath, k => new AsyncLock()).LockAsync())
            {
                return await InternalSendAsync();
            }

            async Task<HttpResponseMessage> InternalSendAsync()
            {
                var retryTimes = 0;
                WebApiRequestInfo info = null;
                var scope = Lifestyle.Scoped.GetCurrentScope(_container);
                do
                {
                    try
                    {
                        // Occurs on retry
                        //if (info != null)
                        //{
                        //    if (info.Transaction.RetryDelay > 0)
                        //    {
                        //        await Task.Delay(TimeSpan.FromMilliseconds(info.Transaction.RetryDelay), cancellationToken);
                        //    }

                        //    var newInfo = CreateRequestInfo(info.Transaction);
                        //    foreach (var pair in info.Sessions)
                        //    {
                        //        var session = pair.Value.SessionWithOptions()
                        //            .AutoJoinTransaction()
                        //            .AutoClose()
                        //            .FlushMode()
                        //            .Interceptor()
                        //            .ConnectionReleaseMode()
                        //            .OpenSession();
                        //        SetupSession(session, info.Transaction, request.GetActionDescriptor());
                        //        newInfo.Sessions.Add(pair.Key, session);
                        //    }
                        //    info = newInfo;
                        //    scope.SetItem(ScopeKey, info);
                        //}

                        var result = await base.SendAsync(request, cancellationToken);
                        info = GetWebApiRequestInfo();
                        if (info == null)
                        {
                            return result;
                        }

                        if (!result.IsSuccessStatusCode)
                        {
                            Rollback(info, retryTimes > 0);
                            return result;
                        }
                        await CommitAsync(info, retryTimes > 0);
                        return result;
                    }
                    //catch (StaleStateException e)
                    //{
                    //    info = GetWebApiRequestInfo();
                    //    if (info == null)
                    //    {
                    //        throw;
                    //    }
                    //    Rollback(info, retryTimes > 0);
                    //    if (retryTimes >= info.Transaction.RetryTimes)
                    //    {
                    //        _logger.ErrorException(
                    //            retryTimes == 0
                    //                ? "Unable to successfully complete the operation, try to use the RetryTimes option to retry the operation."
                    //                : $"Unable to successfully complete the operation after {retryTimes} retries, try to use the Sequential flag in order to avoid concurrent transactions.",
                    //            e);
                    //        throw;
                    //    }
                    //    _logger.WarnException($"Concurrent transactions are trying to update the same data...retying the operation after {info.Transaction.RetryDelay}ms. Total retries: {retryTimes}.", e);
                    //    retryTimes++;
                    //    continue;
                    //}
                    catch
                    {
                        info = GetWebApiRequestInfo();
                        if (info != null)
                        {
                            Rollback(info, retryTimes > 0);
                        }
                        throw;
                    }
                    finally
                    {
                        if (info != null)
                        {
                            scope.SetItem(ScopeKey, null);
                        }
                    }
                } while (true);
            }
        }

        private WebApiRequestInfo CreateRequestInfo(TransactionAttribute transaction)
        {
            var transactionScope = transaction.Distributed
                ? new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)
                : null;
            return new WebApiRequestInfo(transaction, transactionScope);
        }

        protected virtual bool IsReadOnly(HttpRequestMessage requestMessage, HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor?.GetCustomAttributes<ReadOnlyAttribute>().SingleOrDefault()?.IsReadOnly == true;
        }

        protected virtual void SetupSession(ISession session, TransactionAttribute transaction, HttpRequestMessage requestMessage, HttpActionDescriptor actionDescriptor)
        {
            if (transaction.Enabled)
            {
                if (transaction.Level.HasValue)
                {
                    session.BeginTransaction(transaction.Level.Value);
                }
                else
                {
                    session.BeginTransaction();
                }
            }

            if (IsReadOnly(requestMessage, actionDescriptor))
            {
                session.DefaultReadOnly = true;
                session.FlushMode = FlushMode.Manual;
            }
        }

        private void Rollback(WebApiRequestInfo info, bool dispose)
        {
            if (info.TransactionScope != null)
            {
                info.TransactionScope.Dispose();
            }
            else
            {
                foreach (var session in info.Sessions.Values)
                {
                    session.RollbackTransaction();
                    if (dispose)
                    {
                        session.Dispose();
                    }
                }
            }
        }

        private async Task CommitAsync(WebApiRequestInfo info, bool dispose)
        {
            foreach (var session in info.Sessions.Values)
            {
                await session.CommitTransactionAsync();
                if (dispose)
                {
                    session.Dispose();
                }
            }

            info.TransactionScope?.Complete();
            info.TransactionScope?.Dispose();
        }

        private WebApiRequestInfo GetWebApiRequestInfo()
        {
            var dictObj = Lifestyle.Scoped.GetCurrentScope(_container)?.GetItem(ScopeKey);
            return dictObj as WebApiRequestInfo;
        }
    }
}
