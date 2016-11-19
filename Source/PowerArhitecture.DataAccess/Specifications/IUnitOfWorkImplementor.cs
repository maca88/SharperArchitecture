using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IUnitOfWorkImplementor : IUnitOfWork
    {
        /// <summary>
        /// Get the session for the default database configuration.
        /// The session will be created if is not yet
        /// </summary>
        ISession DefaultSession { get; }

        /// <summary>
        /// Check whether the session for the default database configuration was created.
        /// </summary>
        bool ContainsDefaultSession();

        /// <summary>
        /// Check whether the session for the database configuration name was created.
        /// </summary>
        bool ContainsSession(string dbConfigName);

        /// <summary>
        /// Use this if you want instantiate a class that has a Repository or Session as dependency
        /// </summary>
        IInstanceProvider ResolutionRoot { get; }

        IEnumerable<ISession> GetActiveSessions();
    }
}
