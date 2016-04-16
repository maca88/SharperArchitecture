using System;

namespace PowerArhitecture.CodeList.Attributes
{
    public class CodeListAttribute : Attribute
    {
        public CodeListAttribute()
        {
            CodeLength = 20;
            AddTablePrefix = true;
            ManipulateIdentifier = true;
        }

        public string ViewName { get; set; }

        public int CodeLength { get; set; }

        public int NameLength { get; set; }

        public bool Cache { get; set; }

        public bool ManipulateIdentifier { get; set; }

        public bool AddTablePrefix { get; set; }
    }
}
