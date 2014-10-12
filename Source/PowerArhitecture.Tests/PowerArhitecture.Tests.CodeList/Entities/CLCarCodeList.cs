
using System;
using PowerArhitecture.CodeList;
using PowerArhitecture.CodeList.Attributes;
using FluentNHibernate.Automapping;

namespace PowerArhitecture.Tests.CodeList.Entities
{
    [Serializable]
    [GenerateCodeList]
    public partial class CLCarCodeList : CodeListLoc<CLCarCodeList, CLCarCodeListLocalization>
    {
    }

    [Serializable]
    public partial class CLCarCodeListLocalization : CodeListLocalization<CLCarCodeList, CLCarCodeListLocalization>
    {
    }
}
