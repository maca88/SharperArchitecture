using System;

namespace PowerArhitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InclusiveBetweenAttribute : ValidationAttribute
    {
        public InclusiveBetweenAttribute(IComparable from, IComparable to)
        {
            From = from;
            To = to;
        }

        public IComparable From { get; private set; }

        public IComparable To { get; private set; }
    }
}
