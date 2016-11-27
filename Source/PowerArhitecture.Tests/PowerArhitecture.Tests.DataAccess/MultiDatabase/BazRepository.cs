using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Tests.DataAccess.MultiDatabase
{
    public interface IBazRepository<TModel> : IBazRepository<TModel, long> where TModel : class, IEntity<long>, new()
    {
        
    }

    public interface IBazRepository<TModel, TId> : IRepository<TModel, TId> where TModel : class, IEntity<TId>, new()
    {
    }

    public class BazRepository<TModel> : BazRepository<TModel, long>, IBazRepository<TModel>
        where TModel : class, IEntity<long>, new()
    {
        public BazRepository(ISession session, ILogger logger) : base(session, logger)
        {
        }
    }

    public class BazRepository<TModel, TId> : Repository<TModel, TId>, IBazRepository<TModel, TId>
        where TModel : class, IEntity<TId>, new()
    {
        public BazRepository(ISession session, ILogger logger) : base(session, logger)
        {
        }
    }
}
