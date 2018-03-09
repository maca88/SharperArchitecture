using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;
using SharperArchitecture.Common.Queries;
using SharperArchitecture.DataAccess.Specifications;
using SharperArchitecture.Tests.DataAccess.Entities;

namespace SharperArchitecture.Tests.DataAccess.Queries
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

        public Task<List<User>> HandleAsync(GetAllUsersAsyncQuery query, CancellationToken cancellationToken)
        {
            return _dbStore.Query<User>().ToListAsync(cancellationToken);
        }
    }
}
