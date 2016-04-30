using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IAuditUserProvider
    {
        Task<object> GetCurrentUser(ISession session, Type userType);
    }
}
