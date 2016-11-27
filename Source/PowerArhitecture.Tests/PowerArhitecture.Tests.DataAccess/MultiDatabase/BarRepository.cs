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
    public interface IBarRepository<TModel, TId> : IRepository<TModel, TId> where TModel : class, IEntity<TId>, new()
    {
    }

    public class BarRepository<TModel, TId> : Repository<TModel, TId>, IBarRepository<TModel, TId>
        where TModel : class, IEntity<TId>, new()
    {
        public BarRepository(ISession session, ILogger logger) : base(session, logger)
        {
        }
    }
}
