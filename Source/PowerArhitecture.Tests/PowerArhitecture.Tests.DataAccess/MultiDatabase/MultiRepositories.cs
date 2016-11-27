using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Tests.DataAccess.Entities;

namespace PowerArhitecture.Tests.DataAccess.MultiDatabase
{
    public class MultiGenericRepositories
    {
        public MultiGenericRepositories(IRepository<AttrIndexAttribute> repository, IBarRepository<AttrIndexAttribute, long> barRepository)
        {
            Repository = repository;
            BarRepository = barRepository;
        }

        public IRepository<AttrIndexAttribute> Repository { get; }

        public IBarRepository<AttrIndexAttribute, long> BarRepository { get; }
    }
}
