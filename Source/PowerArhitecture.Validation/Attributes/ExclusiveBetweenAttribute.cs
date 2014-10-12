using System;

namespace PowerArhitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExclusiveBetweenAttribute : ValidationAttribute
    {
        public ExclusiveBetweenAttribute(IComparable from, IComparable to)
        {
            From = from;
            To = to;
        }

        public IComparable From { get; private set; }

        public IComparable To { get; private set; }
    }
}
