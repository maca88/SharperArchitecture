using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PowerArhitecture.Domain;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IUnitOfWork : IDbStore, IDisposable
    {
        void Flush();

        Task FlushAsync();

        void Commit();

        Task CommitAsync();

        void Rollback();

        IUnitOfWorkImplementor GetUnitOfWorkImplementation();
    }
}
