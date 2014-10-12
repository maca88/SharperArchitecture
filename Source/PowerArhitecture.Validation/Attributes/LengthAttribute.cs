using System;

namespace PowerArhitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LengthAttribute : ValidationAttribute
    {
        public LengthAttribute(int max) :this(int.MinValue, max)
        {
        }

        public LengthAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; private set; }

        public int Max { get; private set; }

        public bool IsMinSet()
        {
            return Min != int.MinValue;
        }
    }
}
