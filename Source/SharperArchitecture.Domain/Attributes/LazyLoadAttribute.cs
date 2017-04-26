using System;

namespace SharperArchitecture.Domain.Attributes
{
    public class LazyLoadAttribute : Attribute
    {
        public LazyLoadAttribute(bool enable = true)
        {
            Enabled = enable;
        }

        public bool Enabled { get; private set; }
    }
}
