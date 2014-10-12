using PowerArhitecture.Domain;

namespace PowerArhitecture.CodeList.Specifications
{
    public interface ICodeList : IEntity
    {
        string Code { get; set; }

        bool Active { get; set; }
    }
}
