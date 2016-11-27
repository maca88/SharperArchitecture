using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using FluentNHibernate.Automapping;
using NHibernate;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Tests.DataAccess.MultiDatabase;

namespace PowerArhitecture.Tests.DataAccess.Entities
{
    public partial class AttrLazyLoad : Entity
    {
        public virtual string Name { get; set; }

        [LazyLoad]
        public virtual string Xml { get; set; }
    }

    public interface IAttrLazyLoadRepository : IRepository<AttrLazyLoad>
    {
        
    }

    public class AttrLazyLoadRepository : Repository<AttrLazyLoad>, IAttrLazyLoadRepository
    {
        public AttrLazyLoadRepository(ISession session, ILogger logger) : base(session, logger)
        {
        }
    }

    public interface IBarAttrLazyLoadRepository : IBarRepository<AttrLazyLoad, long>
    {
    }

    public class BarAttrLazyLoadRepository : BarRepository<AttrLazyLoad, long>, IBarAttrLazyLoadRepository
    {
        public BarAttrLazyLoadRepository(ISession session, ILogger logger) : base(session, logger)
        {
        }
    }
}
