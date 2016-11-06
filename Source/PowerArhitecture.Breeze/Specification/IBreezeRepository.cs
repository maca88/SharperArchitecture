using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.NH;
using Newtonsoft.Json.Linq;

namespace PowerArhitecture.Breeze.Specification
{
    public interface IBreezeRepository : IDisposable
    {
        object[] GetKeyValues(object entity);

        NhQueryableInclude<T> GetQuery<T>(bool cacheable = false);

        string Metadata();

        IKeyGenerator KeyGenerator { get; set; }

        SaveOptions SaveOptions { get; set; }

        SaveResult SaveChanges(JObject saveBundle, TransactionSettings transactionSettings);

        SaveResult SaveChanges(JObject saveBundle, TransactionSettings transactionSettings, Action<List<EntityInfo>> beforeSaveFunc);

        Task<SaveResult> SaveChangesAsync(JObject saveBundle, TransactionSettings transactionSettings);

        Task<SaveResult> SaveChangesAsync(JObject saveBundle, TransactionSettings transactionSettings, Func<List<EntityInfo>, Task> beforeSaveFunc);

        SaveResult SaveChanges(JObject saveBundle);

        SaveResult SaveChanges(JObject saveBundle, Action<List<EntityInfo>> beforeSaveFunc);

        Task<SaveResult> SaveChangesAsync(JObject saveBundle);

        Task<SaveResult> SaveChangesAsync(JObject saveBundle, Func<List<EntityInfo>, Task> beforeSaveFunc);
    }
}
