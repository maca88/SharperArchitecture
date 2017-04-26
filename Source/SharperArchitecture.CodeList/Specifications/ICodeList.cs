using SharperArchitecture.Domain;

namespace SharperArchitecture.CodeList.Specifications
{
    public interface ICodeList : IEntity
    {
        string Code { get; set; }

        bool Active { get; set; }
    }
}
