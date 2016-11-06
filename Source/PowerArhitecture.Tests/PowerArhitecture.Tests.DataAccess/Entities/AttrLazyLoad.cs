using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using FluentNHibernate.Automapping;
using NHibernate;
using Ninject.Extensions.Logging;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public partial class AttrLazyLoad : Entity
    {
        public virtual string Name { get; set; }

        [LazyLoad]
        public virtual string Xml { get; set; }
    }

    public interface IAttrLazyLoadRepository : IRepository
    {
        
    }

    public class AttrLazyLoadRepository : Repository<AttrLazyLoad>, IAttrLazyLoadRepository
    {
        public AttrLazyLoadRepository(Lazy<ISession> session, ILogger logger) : base(session, logger)
        {
        }
    }
}
