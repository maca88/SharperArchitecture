using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Notifications.Specifications;

namespace PowerArhitecture.Notifications.SearchQueries
{
    public class RoleSearchQuery : IRecipientSearchQuery
    {
        private readonly IRepository<Role> _roleRepository;

        public RoleSearchQuery(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public IEnumerable<IUser> GetRecipients(string pattern)
        {
            return _roleRepository.GetLinqQuery()
                .Where(o => o.Name == pattern)
                .SelectMany(o => o.UserRoles.Select(u => u.User))
                .ToList();
        }
    }
}
