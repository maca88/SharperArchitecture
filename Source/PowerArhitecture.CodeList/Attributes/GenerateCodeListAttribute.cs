using System;

namespace PowerArhitecture.CodeList.Attributes
{
    public class GenerateCodeListAttribute : Attribute
    {
        public GenerateCodeListAttribute()
        {
            CodeLength = 20;
        }

        public string ViewName { get; set; }

        public int CodeLength { get; set; }
    }
}
