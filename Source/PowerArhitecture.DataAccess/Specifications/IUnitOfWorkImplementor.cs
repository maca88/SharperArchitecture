using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IUnitOfWorkImplementor : IUnitOfWork
    {
        ISession Session { get; }

        /// <summary>
        /// Use this if you want instantiate a class that has a Repository or Session as dependency
        /// </summary>
        IResolutionRoot ResolutionRoot { get; }
    }
}
