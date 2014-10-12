using System;
using System.Collections.Generic;

namespace PowerArhitecture.CodeList.Specifications
{
    public interface ICodeListCache
    {
        void UpdateOrInsertCodeLists(IEnumerable<Type> codeListTypes);

        IEnumerable<TType> GetCodeList<TType>() where TType : ICodeList;

        TType GetCodeList<TType>(string code) where TType : ICodeList;

        void UpdateOrInsertCodeList(Type codeListType);

        void UpdateOrInsertCodeList<TType>();
    }
}
