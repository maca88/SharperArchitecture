using System;

namespace PowerArhitecture.Common.Attributes
{
    public class PriorityAttribute : Attribute
    {
        public const short Default = 0;

        public PriorityAttribute(short priority)
        {
            Priority = priority;
        }

        public short Priority { get; private set; }
    }
}
