using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IUnitOfWorkImplementor
    {
        ISession Session { get; }

        IResolutionRoot ResolutionRoot { get; }
    }
}
