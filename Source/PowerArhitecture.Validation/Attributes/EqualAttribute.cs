using System;

namespace PowerArhitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EqualAttribute : ComparisonAttribute
    {
        public EqualAttribute() { }

        public EqualAttribute(object value) : base(value)
        {
        }
    }
}
