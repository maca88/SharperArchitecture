using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Tests.DataAccess.Entities;

namespace PowerArhitecture.Tests.DataAccess.Queries
{
    public class GetAllUsersAsyncQuery : IAsyncQuery<List<User>>
    {
    }

    public class GetAllUsersAsyncQueryHandler : IAsyncQueryHandler<GetAllUsersAsyncQuery, List<User>>
    {
        private readonly IDbStore _dbStore;

        public GetAllUsersAsyncQueryHandler(IDbStore dbStore)
        {
            _dbStore = dbStore;
        }

        public Task<List<User>> HandleAsync(GetAllUsersAsyncQuery query)
        {
            return _dbStore.Query<User>().ToListAsync();
        }
    }
}
