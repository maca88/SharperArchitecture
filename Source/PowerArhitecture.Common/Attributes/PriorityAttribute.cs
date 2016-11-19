using System;

namespace PowerArhitecture.Common.Attributes
{
    public class PriorityAttribute : Attribute
    {
        public const ushort Default = 10;

        public PriorityAttribute(ushort priority)
        {
            Priority = priority;
        }

        public ushort Priority { get; private set; }
    }
}
