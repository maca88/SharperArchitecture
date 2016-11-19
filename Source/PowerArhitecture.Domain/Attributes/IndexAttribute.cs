using System;

namespace PowerArhitecture.Domain.Attributes
{
    public class IndexAttribute : Attribute
    {
        public IndexAttribute(string keyName)
        {
            KeyName = keyName;
        }

        public IndexAttribute()
        { 
        }

        public string KeyName { get; }

        public bool IsKeySet => !string.IsNullOrEmpty(KeyName);
    }
}
