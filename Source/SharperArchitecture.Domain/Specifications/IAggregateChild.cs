
namespace SharperArchitecture.Domain.Specifications
{
    public interface IAggregateChild
    {
        IVersionedEntity AggregateRoot { get; }
    }
}