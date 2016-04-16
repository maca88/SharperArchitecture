
using System;
using PowerArhitecture.CodeList;
using PowerArhitecture.CodeList.Attributes;
using FluentNHibernate.Automapping;

namespace PowerArhitecture.Tests.CodeList.Entities
{
    [Serializable]
    [CodeList]
    public partial class CLCarCodeList : CodeListLocWithUser<string, CLCarCodeList, CLCarCodeListLocalization>
    {
    }

    [Serializable]
    public partial class CLCarCodeListLocalization : CodeListLocalization<CLCarCodeList, CLCarCodeListLocalization>
    {
    }
}
