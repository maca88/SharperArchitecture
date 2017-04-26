using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.NH;
using Newtonsoft.Json.Linq;

namespace SharperArchitecture.Breeze.Specification
{
    public interface IBreezeContext : IDisposable
    {
        NhQueryableInclude<T> GetQuery<T>(bool cacheable = false);

        string Metadata();

        SaveResult SaveChanges(JObject saveBundle);

        SaveResult SaveChanges(JObject saveBundle, TransactionSettings transactionSettings);

        SaveResult SaveChanges(JObject saveBundle, SaveInterceptorSettings interceptorSettings);

        SaveResult SaveChanges(JObject saveBundle, TransactionSettings transactionSettings, SaveInterceptorSettings interceptorSettings);


        Task<SaveResult> SaveChangesAsync(JObject saveBundle);

        Task<SaveResult> SaveChangesAsync(JObject saveBundle, TransactionSettings transactionSettings);

        Task<SaveResult> SaveChangesAsync(JObject saveBundle, AsyncSaveInterceptorSettings interceptorSettings);

        Task<SaveResult> SaveChangesAsync(JObject saveBundle, TransactionSettings transactionSettings, AsyncSaveInterceptorSettings interceptorSettings);
    }
}
