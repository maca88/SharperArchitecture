using System;

namespace SharperArchitecture.Domain.Attributes
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

        public string KeyName { get; }

        public bool IsKeySet => !string.IsNullOrEmpty(KeyName);
    }
}
