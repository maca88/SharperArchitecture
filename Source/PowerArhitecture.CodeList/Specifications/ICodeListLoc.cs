using System.Collections.Generic;

namespace PowerArhitecture.CodeList.Specifications
{
    public interface ICodeListLoc<TCodeList, TCodeListNames> : ICodeList
        where TCodeList : ICodeListLoc<TCodeList, TCodeListNames>
        where TCodeListNames : ICodeListLocalization<TCodeList, TCodeListNames>
    {
        ISet<TCodeListNames> Names { get; }

        void AddName(TCodeListNames name);

        void RemoveName(TCodeListNames name);
    }
}
