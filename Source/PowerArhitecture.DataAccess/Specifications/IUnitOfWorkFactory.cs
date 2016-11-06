using System.Data;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GetNew(IsolationLevel isolationLevel = IsolationLevel.Unspecified, string dbConfigName = null);
    }
}
