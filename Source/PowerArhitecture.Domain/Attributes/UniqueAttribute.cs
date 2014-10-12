using System;

namespace PowerArhitecture.Domain.Attributes
{
    public class UniqueAttribute : Attribute
    {
        public UniqueAttribute(string keyName)
        {
            KeyName = keyName;
        }

        public UniqueAttribute()
        { 
        }

        public string KeyName { get; private set; }

        public bool IsKeySet { get { return !string.IsNullOrEmpty(KeyName); } }
    }
}
