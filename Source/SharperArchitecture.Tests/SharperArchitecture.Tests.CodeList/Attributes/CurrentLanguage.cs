using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.CodeList.Attributes;

namespace SharperArchitecture.Tests.CodeList.Attributes
{
    public class CurrentLanguageAttribute : FilterCurrentLanguageAttribute
    {
        public CurrentLanguageAttribute(string columnName = null) : base("Language", "Current", "Fallback", columnName)
        {
        }
    }
}
