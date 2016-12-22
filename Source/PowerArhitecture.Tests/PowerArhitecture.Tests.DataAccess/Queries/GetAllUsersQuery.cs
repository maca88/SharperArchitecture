using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Tests.DataAccess.Entities;

namespace PowerArhitecture.Tests.DataAccess.Queries
{
    public class GetAllUsersQuery : IQuery<IEnumerable<User>>
    {
    }

    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IEnumerable<User>>
    {
        private readonly IDbStore _dbStore;

        public GetAllUsersQueryHandler(IDbStore dbStore)
        {
            _dbStore = dbStore;
        }

        public IEnumerable<User> Handle(GetAllUsersQuery query)
        {
            return _dbStore.Query<User>().ToList();
        }
    }
}
