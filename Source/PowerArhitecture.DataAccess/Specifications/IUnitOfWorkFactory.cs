using System.Data;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
    }
}
