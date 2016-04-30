using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Breeze.ContextProvider.NH;
using NHibernate;
using NHibernate.Linq;

namespace PowerArhitecture.Breeze
{
    public class PABreezeController : ApiController
    {
        protected internal static readonly MethodInfo SessionQueryMethod = typeof(LinqExtensionMethods).GetMethods()
            .First(o => o.Name == "Query" && o.GetParameters().First().ParameterType == typeof(ISession));

        public PABreezeController(ISession session)
        {
            Session = session;
            NHibernateContractResolver.Instance.RegisterSessionFactory(session.SessionFactory);
        }

        protected internal ISession Session { get; set; }

        public void RollbackTransaction()
        {
            if (Session.Transaction == null || Session.Transaction.WasRolledBack || !Session.Transaction.IsActive)
                return;
            Session.Transaction.Rollback();
            Session.Transaction.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            if (Session.Transaction.WasRolledBack || !Session.Transaction.IsActive)
                return;
            await Session.Transaction.RollbackAsync().ConfigureAwait(false);
            Session.Transaction.Dispose();
        }

        public bool CommitTransaction()
        {
            if (Session.Transaction.WasRolledBack || !Session.Transaction.IsActive)
                return false;
            try
            {
                Session.Transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                RollbackTransaction();
                throw;
            }
        }
        public async Task<bool> CommitTransactionAsync()
        {
            if (Session.Transaction.WasRolledBack || !Session.Transaction.IsActive)
                return false;
            try
            {
                await Session.Transaction.CommitAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                await RollbackTransactionAsync().ConfigureAwait(false);
                throw;
            }
        }
    }
}
