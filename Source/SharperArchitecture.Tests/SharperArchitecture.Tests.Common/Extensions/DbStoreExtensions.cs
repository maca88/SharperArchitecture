using System.Collections.Generic;
using System.Threading.Tasks;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Tests.Common.Extensions
{
    public static class DbStoreExtensions
    {
        public static void Save(this IDbStore dbStore, IEnumerable<IEntity> models)
        {
            foreach (var model in models)
            {
                dbStore.Save(model);
            }
        }

        public static async Task SaveAsync(this IDbStore dbStore, IEnumerable<IEntity> models)
        {
            foreach (var model in models)
            {
                await dbStore.SaveAsync(model);
            }
        }
    }
}
