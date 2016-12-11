using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NHibernate;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.WebApi.Specifications;
using SimpleInjector;
using System.Transactions;

namespace PowerArhitecture.WebApi.Internal
{
    /// <summary>
    /// Responsable for managing a transaction inside a WebApi request.
    /// </summary>
    [Priority(short.MaxValue)]
    internal class TransactionManager : IEventHandler<SessionCreatedEvent>, IActionFilter
    {
        private readonly IRequestMessageProvider _requestMessageProvider;
        private readonly Container _container;
        private const string ScopeKey = "WebApiReqeustInfo";

        private class WebApiRequestInfo
        {
            public WebApiRequestInfo(TransactionScope transaction = null)
            {
                Transaction = transaction;
            }

            public Dictionary<string, ISession> Sessions { get; } = new Dictionary<string, ISession>();

            public TransactionScope Transaction { get; }
        }

        public TransactionManager(IRequestMessageProvider requestMessageProvider, Container container)
        {
            _requestMessageProvider = requestMessageProvider;
            _container = container;
        }

        public void Handle(SessionCreatedEvent @event)
        {
            var currentMessage = _requestMessageProvider.CurrentMessage;
            var scope = Lifestyle.Scoped.GetCurrentScope(_container);
            if (currentMessage == null || scope == null || scope.IsOwnedByUnitOfWork())
            {
                return;
            }

            var info = GetWebApiRequestInfo();
            if (info == null)
            {
                var transaction = Database.MultipleDatabases
                    ? new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)
                    : null;
                info = new WebApiRequestInfo(transaction);
                scope.SetItem(ScopeKey, info);
            }

            if (info.Sessions.ContainsKey(@event.DatabaseConfigurationName))
            {
                throw new NotSupportedException("Having multiple sessions for one database configuration within the same WebApi request is not supported. " +
                                                $"Multiple sessions detected for database configuration with name: {@event.DatabaseConfigurationName}. " +
                                                "Hint: method container.BeginExecutionContextScope must not be called inside a WebApi request");
            }

            var actionDescriptor = currentMessage.GetActionDescriptor();
            var attr = actionDescriptor.GetCustomAttributes<IsolationLevelAttribute>().SingleOrDefault();
            if (attr != null)
            {
                @event.Session.BeginTransaction(attr.Level);
            }
            else
            {
                @event.Session.BeginTransaction();
            }
            info.Sessions.Add(@event.DatabaseConfigurationName, @event.Session);
        }

        public bool AllowMultiple => false;

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, 
            Func<Task<HttpResponseMessage>> continuation)
        {
            WebApiRequestInfo info = null;
            try
            {
                var result = await continuation();
                info = GetWebApiRequestInfo();
                if (info == null)
                {
                    return result;
                }
                foreach (var session in info.Sessions.Values)
                {
                    await session.CommitTransactionAsync();
                }
                info.Transaction?.Complete();
                info.Transaction?.Dispose();
                return result;
            }
            catch
            {
                info = GetWebApiRequestInfo();
                if (info == null)
                {
                    throw;
                }
                if (info.Transaction != null)
                {
                    info.Transaction.Dispose();
                }
                else
                {
                    foreach (var session in info.Sessions.Values)
                    {
                        session.RollbackTransaction();
                    }
                }
                throw;
            }
            finally
            {
                var scope = Lifestyle.Scoped.GetCurrentScope(_container);
                if (info != null)
                {
                    scope.SetItem(ScopeKey, null);
                }
            }
        }

        private WebApiRequestInfo GetWebApiRequestInfo()
        {
            var dictObj = Lifestyle.Scoped.GetCurrentScope(_container)?.GetItem(ScopeKey);
            return dictObj as WebApiRequestInfo;
        }
    }
}
