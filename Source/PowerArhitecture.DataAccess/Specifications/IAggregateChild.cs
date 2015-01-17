namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IAggregateChild
    {
        IAggregateRoot AggregateRoot { get; }
    }
}