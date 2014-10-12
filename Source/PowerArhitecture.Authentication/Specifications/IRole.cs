namespace PowerArhitecture.Authentication.Specifications
{
    public interface IRole
    {
        long Id { get; }

        string Name { get; }

        string Description { get; set; }
    }
}
