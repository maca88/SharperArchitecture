using System;

namespace SharperArchitecture.CodeList.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CodeListConfigurationAttribute : Attribute
    {
        public CodeListConfigurationAttribute()
        {
            CodeLength = 20;
            CodeListPrefix = true;
            ManipulateIdentifier = true;
        }

        public int CodeLength { get; set; }

        public int? NameLength { get; set; }

        public bool ManipulateIdentifier { get; set; }

        public bool CodeListPrefix { get; set; }
    }
}
