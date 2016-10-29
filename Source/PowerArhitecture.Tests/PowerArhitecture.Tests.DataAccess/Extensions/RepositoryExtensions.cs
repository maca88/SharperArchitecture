using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.Tests.DataAccess.Extensions
{
    public static class RepositoryExtensions
    {
        public static ISession GetSession(this IRepository repository)
        {
            return (ISession) repository.GetMemberValue("Session");
        }
    }
}
