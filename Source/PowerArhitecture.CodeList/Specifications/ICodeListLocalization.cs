using PowerArhitecture.Domain;

namespace PowerArhitecture.CodeList.Specifications
{
    public interface ICodeListLocalization
    {
        string LanguageCode { get; set; }

        string Name { get; set; }

        object GetCodeList();
    }

    public interface ICodeListLocalization<TCodeList, TCodeListNames> : ICodeListLocalization, IEntity
        where TCodeList : ICodeList
        where TCodeListNames : ICodeListLocalization<TCodeList, TCodeListNames>
    {
        TCodeList CodeList { get; set; }

        void SetCodeList(TCodeList codeList);

        void UnsetCodeList();
    }
}
