using System;

namespace PowerArhitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotEqualAttribute : ComparisonAttribute
    {
        public NotEqualAttribute() { }

        public NotEqualAttribute(object value) : base(value)
        {
        }
    }
}
