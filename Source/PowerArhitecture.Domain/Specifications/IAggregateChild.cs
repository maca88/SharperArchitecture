
namespace PowerArhitecture.Domain.Specifications
{
    public interface IAggregateChild
    {
        IVersionedEntity AggregateRoot { get; }
    }
}