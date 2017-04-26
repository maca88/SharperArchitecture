namespace SharperArchitecture.Domain.Specifications
{
    public interface IAutoValidated
    {
        bool ValidateOnUpdate { get; }

        bool ValidateOnInsert { get; }

        bool ValidateOnDelete { get; }
    }
}
