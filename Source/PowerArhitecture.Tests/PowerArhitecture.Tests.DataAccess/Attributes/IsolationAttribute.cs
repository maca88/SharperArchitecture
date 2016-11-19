using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.Tests.DataAccess.Attributes
{
    public class ReadCommited
    {
        public ReadCommited([IsolationLevel(IsolationLevel.ReadCommitted)] IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; }
    }

    public class Chaos
    {
        public Chaos([IsolationLevel(IsolationLevel.Chaos)] IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; }
    }
}
