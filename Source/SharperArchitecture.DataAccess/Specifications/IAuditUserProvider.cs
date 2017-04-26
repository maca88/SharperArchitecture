using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface IAuditUserProvider
    {
        object GetCurrentUser(ISession session, Type userType);

        Task<object> GetCurrentUserAsync(ISession session, Type userType);
    }
}
