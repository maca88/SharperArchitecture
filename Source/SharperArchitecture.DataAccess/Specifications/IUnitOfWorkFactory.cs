using System.Data;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
    }
}
