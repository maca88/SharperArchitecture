using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.CodeList.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterCurrentLanguageAttribute : Attribute
    {
        public FilterCurrentLanguageAttribute(string filterName, string currentLanguageParameterName, string fallbackLanguageParameterName, 
            string columnName = null)
        {
            FilterName = filterName;
            CurrentLanguageParameterName = currentLanguageParameterName;
            FallbackLanguageParameterName = fallbackLanguageParameterName;
            ColumnName = columnName;
        }

        public string ColumnName { get; set; }

        public string FilterName { get; }

        public string CurrentLanguageParameterName { get; }

        public string FallbackLanguageParameterName { get; }
    }
}
