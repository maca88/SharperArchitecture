using System;

namespace PowerArhitecture.CodeList.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationAttribute : Attribute
    {
        public ConfigurationAttribute()
        {
            CodeLength = 20;
            AddTablePrefix = true;
            ManipulateIdentifier = true;
        }

        public string ViewName { get; set; }

        public int CodeLength { get; set; }

        public int NameLength { get; set; }

        public bool Cacheable { get; set; }

        public bool ManipulateIdentifier { get; set; }

        public bool AddTablePrefix { get; set; }
    }
}
