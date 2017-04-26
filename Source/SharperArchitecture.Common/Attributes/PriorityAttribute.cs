using System;

namespace SharperArchitecture.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PriorityAttribute : Attribute
    {
        public PriorityAttribute(short priority)
        {
            Priority = priority;
        }

        public short Priority { get; private set; }
    }
}
