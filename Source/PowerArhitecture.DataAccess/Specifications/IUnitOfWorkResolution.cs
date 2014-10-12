using Ninject.Syntax;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IUnitOfWorkResolution
    {
        T Get<T>();
    }
}